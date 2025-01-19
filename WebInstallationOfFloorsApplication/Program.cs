using Log;
using Microsoft.EntityFrameworkCore;
using ILogger = Log.ILogger;

namespace WebInstallationOfFloorsApplication {
    public class Program{
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddSingleton<ILogger, ConsoleLog>();
            
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Log");
            if (!Directory.Exists(logDirectory)) {
                Directory.CreateDirectory(logDirectory);
            }
            builder.Services.AddSingleton<ILogger>(provider => new FileLog(Path.Combine(logDirectory, "log" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt")));
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
            
            builder.Services.AddScoped<TaskRepository>();
            builder.Services.AddScoped<TaskService>();
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            logger.Info("Сервисы приложения успешно зарегистрированы");
            
            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope()) {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (dbContext.Database.GetPendingMigrations().Any()) {
                    dbContext.Database.Migrate();
                }
            }

            
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
                logger.Info("Приложение работает в режиме разработки (Development)");
            }
            
            app.UseMiddleware<ErrorHandlingMiddleware>(logger);
            app.UseHttpsRedirection();
            app.MapControllers();

            
            logger.Info("Приложение запущено и готово принимать запросы");
            app.Run();
            
        }
    }
}

