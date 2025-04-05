using System.Security.Claims;
using System.Text;
using Log;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ILogger = Log.ILogger;

namespace WebInstallationOfFloorsApplication {
    public class Program{
        public static async System.Threading.Tasks.Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(MappingConfig).Assembly); 
            var mapper = new Mapper(config);
            builder.Services.AddSingleton<IMapper>(mapper);
            
            builder.Services.AddSingleton<ILogger, ConsoleLog>();
            
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Log");
            if (!Directory.Exists(logDirectory)) {
                Directory.CreateDirectory(logDirectory);
            }
            builder.Services.AddSingleton<ILogger>(provider => new FileLog(Path.Combine(logDirectory, "log" + DateTime.Today.ToString("yyyy-MM-dd") + ".log")));
            builder.Services.AddSingleton<Log.Logger>();
            
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var logger = builder.Services.BuildServiceProvider().GetRequiredService<Log.Logger>();
            logger.Info("Загрузка конфигурации завершена");
            
            string connectionString = builder.Configuration["DB:DBConnection"];
            if (string.IsNullOrEmpty(connectionString)) {
                logger.Error("Не удалось загрузить данные для подключения к бд из конфигурации");
                throw new Exception("Данные подключения к бд отсутствуют в конфигурации!");
            }
            logger.Info("Данные подключения к бд успешно загружены");
            
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            
            string botToken = builder.Configuration["TelegramBot:ApiToken"];
            if (string.IsNullOrEmpty(botToken)) {
                logger.Error("Не удалось загрузить данные телеграм бота из конфигурации");
                throw new Exception("Данные телеграм бота отсутствуют в конфигурации!");
            }
            logger.Info("Данные телеграм бота успешно загружены");
            
            string ngrokUrl = builder.Configuration["ngrok:url"];
            if (string.IsNullOrEmpty(ngrokUrl)) {
                logger.Error("Не удалось загрузить url путь ngrok");
                throw new Exception("Url ngrok отсутствует в конфигурации!");
            }
            logger.Info("Url ngrok успешно загружен");

            string secretKey = builder.Configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey)) {
                logger.Error("Не удалось загрузить secretKey");
                throw new Exception("secretKey отсутствует в конфигурации!");
            }
            logger.Info("secretKey успешно загружен");
            
            string issuer = builder.Configuration["Jwt:Issuer"];
            if (string.IsNullOrEmpty(issuer)) {
                logger.Error("Не удалось загрузить issuer");
                throw new Exception("issuer отсутствует в конфигурации!");
            }
            logger.Info("issuer успешно загружен");
            
            string audience = builder.Configuration["Jwt:Audience"];
            if (string.IsNullOrEmpty(audience)) {
                logger.Error("Не удалось загрузить audience");
                throw new Exception("audience отсутствует в конфигурации!");
            }
            logger.Info("audience успешно загружен");
            
            builder.Services.AddScoped<TaskRepository>();
            builder.Services.AddScoped<TaskService>();
            
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<UserService>();
            
            builder.Services.AddScoped<RoleRepository>();
            builder.Services.AddScoped<RoleService>();
            
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<TelegramService>(sp => {
                var taskRepository = sp.GetRequiredService<TaskRepository>();
                var logger = sp.GetRequiredService<Log.Logger>();
                var httpClient = sp.GetRequiredService<HttpClient>();

                return new TelegramService(taskRepository, logger, httpClient, botToken, ngrokUrl);
            });
            
            builder.Services.AddSingleton<IHostedService, TelegramBackgroundService>();
            
            builder.Services.AddScoped<AccountService>();
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,  
                        ValidAudience = audience, 
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        RoleClaimType = ClaimTypes.Role
                    };
                });
            
            builder.Services.AddScoped<TokenService>(sp => {
                return new TokenService(secretKey, issuer, audience);
            });
            
            builder.Services.AddControllers();
            
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowAll", policy => {
                    var env = builder.Environment;
                    if (env.IsDevelopment()) {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                });
            });
            
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebInstallationOfFloorsApplication API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Description = "Введите токен access:",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
                
            });
            logger.Info("Сервисы приложения успешно зарегистрированы ");
            
            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope()) {
                var telegramService = scope.ServiceProvider.GetRequiredService<TelegramService>();
                await telegramService.SetWebhookAsync(CancellationToken.None);
            }

            
            using (var scope = app.Services.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (dbContext.Database.GetPendingMigrations().Any()) {
                    dbContext.Database.Migrate();
                }
            }
            app.UseCors("AllowAll");
            
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
                logger.Info("Приложение работает в режиме разработки (Development)");
            }
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware<ErrorHandlingMiddleware>(logger);
            
            app.MapControllers();
            logger.Info("Приложение запущено и готово принимать запросы");

            app.Run();
            
        }
    }
}

