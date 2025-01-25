using System.Text.Json;
using Log;

namespace WebInstallationOfFloorsApplication;

public class TelegramBackgroundService: BackgroundService {
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;
    private readonly String _botToken;
    private readonly TaskRepository _taskRepository;

    public TelegramBackgroundService(TaskRepository taskRepository, Logger logger, String botToken) {
        _taskRepository = taskRepository;
        _logger = logger;
        _botToken = botToken;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.telegram.org");
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
                
                var tasks = await _taskRepository.GetPendingTasksAsync(stoppingToken);

                foreach (var task in tasks) {
                    if (task.Worker?.ChatId != null) {
                        string message = FormatMessage(task);
                        await SendMessageAsync(task.Worker.ChatId.ToString(), message);

                        _logger.Info($"Отправлено сообщение для {task.Worker.Name}: {message}");
                    }
                    else {
                        _logger.Warning($"Не найден ChatId для работника с ID {task.WorkerId}");
                    }
                }
            }
            catch (Exception ex) {
                _logger.Error($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }
        
    }
    
    private async System.Threading.Tasks.Task SendMessageAsync(string chatId, string text) {
        string url = $"/bot{_botToken}/sendMessage";

        var parameters = new {
            chat_id = chatId,
            text = text,
            parse_mode = "Markdown",
            reply_markup = new {
                inline_keyboard = new[] {
                    new[] {
                        new {
                            text = "Отменено",
                            callback_data = "Reject"
                        },
                        new {
                            text = "Выполнено",
                            callback_data = "Done"
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(parameters);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try {
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode) {
                _logger.Info($"Сообщение отправлено для chatId {chatId}");
            } else {
                _logger.Error($"Ошибка при отправке сообщения для chatId {chatId}: {response.StatusCode}");
            }
        } catch (Exception ex) {
            _logger.Error($"Ошибка при отправке сообщения для chatId {chatId}: {ex.Message}");
        }
    }
    
    private string FormatMessage(Task task) {
        return $"📌 *Задача на {task.Deadline:dd.MM.yyyy HH:mm}*\n\n" +
               $"🔹 *Заголовок:* {task.Title}\n" +
               $"🏠 *Адрес:* {task.Address}\n" +
               $"📝 *Комментарий:* {task.Comment ?? "Нет комментария"}\n";
    }
}