using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Service;

namespace TelegramBot.Controller;

public class TaskController {
    private readonly TaskService _taskService;
    private readonly ILogger<TaskController> _logger;
    
    public TaskController(TaskService taskService, ILogger<TaskController> logger) {
        _taskService = taskService;
        _logger = logger;
    }
    
    public async Task HandleTaskCreationAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        try {
            var chatId = update.Message.Chat.Id;
            _logger.LogInformation($"Начало создания задачи для чата {chatId}");
            await _taskService.HandleUpdateAsync(botClient, update, cancellationToken);
        }
        catch (Exception ex) {
            var chatId = update.Message.Chat.Id;
            _logger.LogError($"Ошибка при создании задачи для чата {chatId}: {ex.Message}");
            await botClient.SendTextMessageAsync(chatId, "Произошла ошибка при создании задачи. Попробуйте позже.", 
                cancellationToken: cancellationToken);
        }
    }
}