using GitHubApiClient;
using Log;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ListRepositories;
using ListRepositories.ConnectionFactory;
using ServiceListRepositories;
using ILogger = Log.ILogger;

namespace WebGitHubApplication {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddSingleton<ILogger, ConsoleLog>();
            builder.Services.AddSingleton<ILogger> (provider => new FileLog("log.txt"));
            builder.Services.AddSingleton<Log.Logger>();
            
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var logger = builder.Services.BuildServiceProvider().GetRequiredService<Log.Logger>();
            logger.Info("Загрузка конфигурации завершена");

            
            string githubToken = builder.Configuration["GitHubApi:Token"];
            if (string.IsNullOrEmpty(githubToken)) {
                logger.Error("Не удалось загрузить токен GitHub из конфигурации");
                throw new Exception("GitHub API Token отсутствует в конфигурации!");
            }
            logger.Info("GitHub API Token загружен успешно");
            
            
            string connectionString = builder.Configuration["DB:Connection"];
            if (string.IsNullOrEmpty(connectionString)) {
                logger.Error("Не удалось загрузить данные для подключения к бд из конфигурации");
                throw new Exception("Данные подключения к бд отсутствуют в конфигурации!");
            }
            logger.Info("Данные подключения к бд успешно загружены");
            builder.Services.AddSingleton<IConnectionFactory>(connection => new ConnectionFactory(connectionString));
            builder.Services.AddSingleton<IGitHubClient>(new GitHubClient(githubToken));
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            
            builder.Services.AddScoped<UserService>();
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            logger.Info("Сервисы приложения успешно зарегистрированы");
            
            var app = builder.Build();
            
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
