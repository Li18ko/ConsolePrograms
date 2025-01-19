using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using TelegramBot.Service;

namespace TelegramBot.Controller;

public class UserStateController {
    private readonly UserStateService _userStateService;
    private readonly ILogger<UserStateController> _logger;

    public UserStateController(UserStateService userStateService, ILogger<UserStateController> logger) {
        _userStateService = userStateService;
        _logger = logger;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        try {
            if (update.Type == UpdateType.Message && update.Message?.Text != null) {
                _logger.LogInformation($"Получено сообщение от пользователя {update.Message.Chat.Id}: {update.Message.Text}");
                await _userStateService.ProcessMessageAsync(botClient, update, cancellationToken);
            }
            else {
                _logger.LogWarning("Получен неподдерживаемый тип обновления.");
            }
        }
        catch (Exception ex) {
            _logger.LogError($"Ошибка обработки обновления: {ex.Message}");
        }
    }

}