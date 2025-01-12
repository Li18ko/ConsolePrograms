using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TelegramBot {
    public class Program { 
        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try {
                IConfiguration configuration = null;

                try {
                    configuration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();
                    Log.Information("Файл конфигурации appsettings.json успешно загружен.");
                }
                catch (Exception ex){
                    Log.Error("Ошибка при загрузке файла appsettings.json: " + ex.Message);
                    throw;
                }

                String connectionString = null;
                try {
                    connectionString = configuration.GetConnectionString("DBConnection");
                    if (string.IsNullOrEmpty(connectionString)) {
                        throw new InvalidOperationException("Ключ 'DBConnection' не найден в 'ConnectionStrings'.");
                    }
                }
                catch (Exception ex) {
                    Log.Error("Ошибка при получении строки подключения: " + ex.Message);
                    throw;
                }
                Log.Information("Строка с информацией для подключения к бд найдена");
                
                
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<IConfiguration>(configuration)  
                    .AddDbContext<AppDbContext>(options => 
                        options.UseNpgsql(connectionString))
                    .BuildServiceProvider();
        
                using (var context = serviceProvider.GetService<AppDbContext>()) {
                    if (context == null) {
                        throw new InvalidOperationException("Контекст базы данных не найден.");
                    }
                    context.Database.Migrate();
                    Log.Information("Миграции успешно применены и база данных создана (если не существовала)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
        
    }

}
