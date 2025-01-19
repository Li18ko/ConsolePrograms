using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Telegram.Bot;
using TelegramBot.Controller;
using TelegramBot.Service;

namespace TelegramBot {
    public class Program { 
        public static async Task Main(string[] args) {
            try {
                var serviceProvider = new ServiceCollection()
                    .AddLogging(builder => {
                        var logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File("Log/log.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();
                        builder.AddSerilog(logger);
                    })
                    .AddSingleton<IConfiguration>(provider => {
                        var configuration = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();
                        return configuration;
                    })  
                    .AddDbContext<AppDbContext>((provider, options) => {
                        var configuration = provider.GetService<IConfiguration>();
                        var connectionString = configuration["DB:DBConnection"];
                        if (string.IsNullOrEmpty(connectionString))
                            throw new InvalidOperationException("Строка подключения не найдена.");
                        options.UseNpgsql(connectionString);
                    })
                    .AddSingleton<ITelegramBotClient>(provider => {
                        var configuration = provider.GetService<IConfiguration>();
                        var apiToken = configuration["TelegramBot:ApiToken"];
                
                        if (string.IsNullOrEmpty(apiToken)) {
                            throw new InvalidOperationException("Токен API бота не найден в конфигурации.");
                        }

                        return new TelegramBotClient(apiToken);
                    })
                    .AddScoped<BotController>()
                    .AddScoped<BotService>()
                    .AddScoped<UserStateController>()
                    .AddScoped<UserStateService>()
                    .AddScoped<TaskController>()
                    .AddScoped<TaskService>()
                    
                    .AddHangfire(config => config.UseMemoryStorage()) // Использование хранилища в памяти
                    .AddHangfireServer()
                    
                    .BuildServiceProvider();
        
                using (var context = serviceProvider.GetService<AppDbContext>()) {
                    if (context == null) {
                        throw new InvalidOperationException("Контекст базы данных не найден.");
                    }
                    context.Database.Migrate();
                }
                
                using (var scope = serviceProvider.CreateScope()) {
                    var taskService = scope.ServiceProvider.GetRequiredService<TaskService>();
                    var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var cancellationToken = new CancellationToken();
                    
                    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                    
                    string groupChatIdStr = configuration["TelegramBotSettings:GroupChatId"];

                    long groupChatId = long.Parse(groupChatIdStr); 

                    recurringJobManager.AddOrUpdate(
                        "SendTasksForTomorrow",
                        () => taskService.SendTasksForTomorrowAsync(botClient, groupChatId, cancellationToken),
                        Cron.Daily(14, 03));

                }
                Console.WriteLine("Сервис запущен.");
                Console.ReadLine();
                
                using (var scope = serviceProvider.CreateScope()) {
                    var botController = scope.ServiceProvider.GetRequiredService<BotController>();
                    var cancellationTokenSource = new CancellationTokenSource();

                    await botController.StartBotAsync(cancellationTokenSource.Token);
                }

            }
            catch (Exception ex) {
               Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }

}
