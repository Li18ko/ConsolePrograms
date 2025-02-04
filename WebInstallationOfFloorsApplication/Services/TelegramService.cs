using System.Text.Json;
using Log;

namespace WebInstallationOfFloorsApplication;

public class TelegramService {
    private readonly Logger _logger;
    private readonly TaskRepository _taskRepository;
    private readonly HttpClient _httpClient;
    private readonly String _botToken;
    private readonly String _ngrokUrl;

    
    public TelegramService(TaskRepository taskRepository, Logger logger, HttpClient httpClient, 
        String botToken, String ngrokUrl) {
        _taskRepository = taskRepository;
        _logger = logger;
        _botToken = botToken;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.telegram.org");
        _ngrokUrl = ngrokUrl;
    }
    
    public async System.Threading.Tasks.Task SetWebhookAsync(CancellationToken cancellationToken) {
        var url = $"{_ngrokUrl}/api/Telegram/webhook";
        string telegramUrl = $"/bot{_botToken}/setWebhook?url={url}";

        var response = await _httpClient.GetAsync(telegramUrl, cancellationToken);
        if (response.IsSuccessStatusCode) {
            _logger.Info("Webhook успешно установлен");
        } else {
            _logger.Error($"Ошибка при установке webhook: {response.StatusCode}");
        }
    }
    
    public async System.Threading.Tasks.Task HandleWebhookUpdateAsync(CallbackQueryDto callbackQueryDto, 
        CancellationToken cancellationToken) {
        var options = new JsonSerializerOptions {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        _logger.Debug($"Была нажата кнопка: {JsonSerializer.Serialize(callbackQueryDto, options)}");
        
        var dataDetails = JsonSerializer.Deserialize<DataDetails>(callbackQueryDto.CallbackQuery.Data);
        if (dataDetails != null) {
            var action = dataDetails.Action;
            var taskId = dataDetails.TaskId;
        
            await _taskRepository.UpdateTaskStatusAsync(taskId,
                action == Action.Done ? TaskStatus.Done : TaskStatus.Reject, cancellationToken);
            _logger.Info($"Статус задачи с ID {taskId} изменен на {action}");

            var taskUpdate = await _taskRepository.GetTaskAsync((int)taskId, cancellationToken);
            string message = FormatMessage(taskUpdate);

            await EditMessageTextAsync(callbackQueryDto.CallbackQuery.Message.Chat.Id, 
                callbackQueryDto.CallbackQuery.Message.MessageId, message, cancellationToken);
        }
    }
    
    public async System.Threading.Tasks.Task SendDailyMessagesAsync(CancellationToken cancellationToken) {
        _logger.Info("Запрос на получение задач на завтрашний день");
        var tasks = (await _taskRepository.GetTasksForTomorrow(cancellationToken)).ToList();
        _logger.Info(tasks.Count != 0 ? "Задачи получены" : "Задач на завтра нет, сообщения не будут отправлены");
        
        foreach (var task in tasks) {
            if (task.Worker?.ChatId != null) {
                string message = FormatMessage(task);
                await SendTelegramMessageAsync(task.Worker.ChatId, message, 
                    task.Status == TaskStatus.Open? task.Id : 0, cancellationToken);
                
                _logger.Info($"Отправлено сообщение для {task.Worker.Name} \n: {message}");
            }
            else {
                _logger.Warning($"Не найден ChatId для работника с ID {task.WorkerId}");
            }
        }
        
    }
    
    private async System.Threading.Tasks.Task SendTelegramMessageAsync(long chatId, string text, long taskId, 
        CancellationToken cancellationToken) {
        string url = $"/bot{_botToken}/sendMessage";

        var parameters = new {
            chat_id = chatId,
            text = text,
            parse_mode = "Markdown",
            reply_markup = new {
                inline_keyboard = taskId == 0 ? new List<List<object>> {} : new List<List<object>> {
                    new List<object> {
                        new { text = "Отменено", callback_data = JsonSerializer.Serialize(new {
                            action = "Reject", 
                            taskId = taskId 
                        })},
                        new { text = "Выполнено", callback_data = JsonSerializer.Serialize(new {
                            action = "Done", 
                            taskId = taskId
                        })}
                    }
                }
            }
        };
        
        await SendPostRequestAsync(parameters, url, cancellationToken);
    }
    
    private async System.Threading.Tasks.Task EditMessageTextAsync(long chatId, long messageId,  string text, 
        CancellationToken cancellationToken) {
        string url = $"/bot{_botToken}/editMessageText";

        var parameters = new {
            chat_id = chatId,
            message_id = messageId,
            text = text,
            parse_mode = "Markdown"
        };
        
        await SendPostRequestAsync(parameters, url, cancellationToken); 
    }

    private async System.Threading.Tasks.Task SendPostRequestAsync(object parameters, string url, 
        CancellationToken cancellationToken) {
        var json = JsonSerializer.Serialize(parameters);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try {
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            if (response.IsSuccessStatusCode) {
                _logger.Info($"Запрос успешно отправлен: {url}");
            } else {
                _logger.Error($"Ошибка при отправке запроса: {url}: {response.StatusCode}");
            }
        } catch (Exception ex) {
            _logger.Error($"Ошибка при отправке запроса {url}: {ex.Message}");
        }
    }
    
    private string FormatMessage(Task task) {
        return (task.Status == TaskStatus.Done ? $"✅ Задача выполнена! \n\n" : "") +
            (task.Status == TaskStatus.Reject ? $"❌ Задача отменена! \n\n" : "") +                                                          
            $"📌 *Задача на {task.Deadline:dd.MM.yyyy HH:mm}*\n\n" +
            $"🔹 *Заголовок:* {task.Title}\n" +
            $"🏠 *Адрес:* {task.Address}\n" +
            $"📝 *Комментарий:* {task.Comment ?? "Нет комментария"}\n";
    }
    
}