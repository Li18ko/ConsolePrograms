using System.Text.Json;
using Log;

namespace WebInstallationOfFloorsApplication;

public class TelegramService {
    private readonly Logger _logger;
    private readonly TelegramRepository _telegramRepository;
    private readonly HttpClient _httpClient;
    private readonly String _botToken;

    
    public TelegramService(TelegramRepository telegramRepository, Logger logger, HttpClient httpClient, String botToken) {
        _telegramRepository = telegramRepository;
        _logger = logger;
        _botToken = botToken;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.telegram.org");
    }
    
    public async System.Threading.Tasks.Task SetWebhookAsync(CancellationToken cancellationToken) {
        var url = $"https://13f3-5-182-87-116.ngrok-free.app/api/Telegram/webhook";
        string telegramUrl = $"/bot{_botToken}/setWebhook?url={url}";

        var response = await _httpClient.GetAsync(telegramUrl, cancellationToken);
        if (response.IsSuccessStatusCode) {
            _logger.Info("Webhook успешно установлен");
        } else {
            _logger.Error($"Ошибка при установке webhook: {response.StatusCode}");
        }
    }
    
    public async System.Threading.Tasks.Task HandleWebhookUpdateAsync(JsonElement update, CancellationToken cancellationToken) {
        if (update.TryGetProperty("callback_query", out JsonElement callbackQuery)) {
            var callbackData = callbackQuery.GetProperty("data").GetString();
            var chatId = callbackQuery.GetProperty("message").GetProperty("chat").GetProperty("id").GetInt64();
            var messageId = callbackQuery.GetProperty("message").GetProperty("message_id").GetInt64();

            var parts = callbackData.Split('_');
            if (parts.Length == 2) {
                var action = parts[0];
                var taskId = long.Parse(parts[1]);

                await _telegramRepository.UpdateTaskStatusAsync(taskId,
                    action == "Done" ? TaskStatus.Done : TaskStatus.Reject, cancellationToken);
                _logger.Info($"Статус задачи с ID {taskId} изменен на {action}");

                var taskUpdate = await _telegramRepository.GetTaskAsync((int)taskId, cancellationToken);

                await EditMessageTextAsync(chatId.ToString(), messageId,
                    action == "Done" ? $"✅ Задача выполнена! \n\n" + FormatMessage(taskUpdate) : 
                        $"❌ Задача отменена! \n\n" + FormatMessage(taskUpdate), cancellationToken);
            }
        }
    }
    
    public async System.Threading.Tasks.Task SendDailyMessagesAsync(CancellationToken cancellationToken) {
        _logger.Info("Запрос на получение задач на завтрашний день");
        var tasks = await _telegramRepository.GetPendingTasksAsync(cancellationToken);
        _logger.Info(tasks.Any() ? "Задачи получены" : "Задач на завтра нет, сообщения не будут отправлены");
        
        foreach (var task in tasks) {
            if (task.Worker?.ChatId != null) {
                string message = FormatMessage(task);
                await SendTelegramMessageAsync(task.Worker.ChatId.ToString(), message, task.Id, cancellationToken);
                
                _logger.Info($"Отправлено сообщение для {task.Worker.Name}: {message}");
            }
            else {
                _logger.Warning($"Не найден ChatId для работника с ID {task.WorkerId}");
            }
        }
        
    }
    
    private async System.Threading.Tasks.Task SendTelegramMessageAsync(string chatId, string text, long taskId, CancellationToken cancellationToken) {
        string url = $"/bot{_botToken}/sendMessage";

        var parameters = new {
            chat_id = chatId,
            text = text,
            parse_mode = "Markdown",
            reply_markup = new {
                inline_keyboard = new List<List<object>> {
                    new List<object> {
                        new { text = "Отменено", callback_data = $"Reject_{taskId}" },
                        new { text = "Выполнено", callback_data = $"Done_{taskId}" }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(parameters);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try {
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            if (response.IsSuccessStatusCode) {
                _logger.Info($"Сообщение отправлено для chatId {chatId}");
            } else {
                _logger.Error($"Ошибка при отправке сообщения для chatId {chatId}: {response.StatusCode}");
            }
        } catch (Exception ex) {
            _logger.Error($"Ошибка при отправке сообщения для chatId {chatId}: {ex.Message}");
        }
    }
    
    private async System.Threading.Tasks.Task EditMessageTextAsync(string chatId, long messageId,  string text, 
        CancellationToken cancellationToken) {
        string url = $"/bot{_botToken}/editMessageText";

        var parameters = new {
            chat_id = chatId,
            message_id = messageId,
            text = text,
            parse_mode = "Markdown"
        };


        var json = JsonSerializer.Serialize(parameters);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try {
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            if (response.IsSuccessStatusCode) {
                _logger.Info($"Сообщение с ID {messageId} обновлено");
            } else {
                _logger.Error($"Ошибка при обновлении сообщения с ID {messageId}: {response.StatusCode}");
            }
        } catch (Exception ex) {
            _logger.Error($"Ошибка при обновлении сообщения с ID {messageId}: {ex.Message}");
        }
    }
    
    private string FormatMessage(Task task) {
        return $"📌 *Задача на {task.Deadline:dd.MM.yyyy HH:mm}*\n\n" +
               $"🔹 *Заголовок:* {task.Title}\n" +
               $"🏠 *Адрес:* {task.Address}\n" +
               $"📝 *Комментарий:* {task.Comment ?? "Нет комментария"}\n";
    }
    
}