using Log;

namespace WebInstallationOfFloorsApplication;

public class TelegramBackgroundService: BackgroundService {
    private readonly IServiceProvider _serviceProvider;
    private readonly Logger _logger;

    public TelegramBackgroundService(IServiceProvider serviceProvider, Logger logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            try {
                DateTime now = DateTime.UtcNow.AddHours(3);
                DateTime nextRun = new DateTime(now.Year, now.Month, now.Day, 17, 0, 0); 
                if (now > nextRun) {
                    nextRun = nextRun.AddDays(1);
                }
                TimeSpan delay = nextRun - now;
                _logger.Info($"Задержка до следующего 17:00: {delay}");
                await System.Threading.Tasks.Task.Delay(delay, stoppingToken);
                
                _logger.Info("Время 17:00! Выполняем задачу...");

                using (var scope = _serviceProvider.CreateScope()) {
                    var telegramService = scope.ServiceProvider.GetRequiredService<TelegramService>();
                    await telegramService.SendDailyMessagesAsync(stoppingToken);
                }
                
            }
            catch (Exception ex) {
                _logger.Error($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }
    }
}