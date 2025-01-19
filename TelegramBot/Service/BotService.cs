using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBot.Controller;

namespace TelegramBot.Service;

public class BotService {
    private ITelegramBotClient _telegramBotClient;
    private readonly ILogger<BotService> _logger;
    private readonly UserStateController _userStateController;
    

    public BotService(ITelegramBotClient telegramBotClient, ILogger<BotService> logger, UserStateController userStateController) {
        _telegramBotClient = telegramBotClient;
        _logger = logger;
        _userStateController = userStateController;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken) {
        var receiverOptions = new ReceiverOptions {
            AllowedUpdates = { } 
        };
        
        _telegramBotClient.StartReceiving(
            (botClient, update, ct) => _userStateController.HandleUpdateAsync(botClient, update, ct),
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        _logger.LogInformation("Бот начал слушать сообщения...");
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
    

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
        _logger.LogError($"Произошла ошибка: {exception.Message}");
        return Task.CompletedTask;
    }

}