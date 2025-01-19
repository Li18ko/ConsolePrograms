using Microsoft.Extensions.Logging;
using TelegramBot.Service;
using Task = System.Threading.Tasks.Task;

namespace TelegramBot.Controller {
    public class BotController {
        private BotService _botService;
        private readonly ILogger<BotController> _logger;

        public BotController(BotService botService, ILogger<BotController> logger) {
            _botService = botService;
            _logger = logger;
        }
        
        public async Task StartBotAsync(CancellationToken cancellationToken) {
            try {
                _logger.LogInformation("Запуск бота...");
                await _botService.StartAsync(cancellationToken);
            }
            catch (Exception ex) {
                _logger.LogError($"Произошла ошибка при запуске бота: {ex.Message}");
            }
        }

        

    }
}