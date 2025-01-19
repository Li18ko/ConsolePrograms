using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using TaskStatus = TelegramBot.Model.TaskStatus;

namespace TelegramBot.Service
{
    public class TaskService {

        private readonly ILogger<TaskService> _logger;
        private readonly AppDbContext _dbContext;
        private readonly Dictionary<long, string> _taskStates = new Dictionary<long, string>();
        private readonly Dictionary<long, Dictionary<string, object>> _taskData = new Dictionary<long, Dictionary<string, object>>();
        

        public TaskService(ILogger<TaskService> logger, AppDbContext dbContext) {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
            var chatId = update.Message.Chat.Id;

            if (!_taskStates.ContainsKey(chatId)) {
                _taskStates[chatId] = "WaitingForTitle"; 
            }

            string taskState = _taskStates[chatId]; 
            switch (taskState) {
                case "WaitingForTitle":
                    await HandleTitleAsync(botClient, chatId, update, cancellationToken);
                    break;

                case "WaitingForComment":
                    await HandleCommentAsync(botClient, chatId, update, cancellationToken);
                    break;

                case "WaitingForAddress":
                    await HandleAddressAsync(botClient, chatId, update, cancellationToken);
                    break;

                case "WaitingForDeadline":
                    await HandleDeadlineAsync(botClient, chatId, update, cancellationToken);
                    break;
            }
        }

        
        private async Task HandleTitleAsync(ITelegramBotClient botClient, long chatId, Update update, CancellationToken cancellationToken) {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Введите название задачи:", cancellationToken: cancellationToken);
            var messageText = update.Message.Text;
            _logger.LogInformation(messageText);

            if (string.IsNullOrEmpty(messageText)) {
                await botClient.SendTextMessageAsync(chatId, "Название задачи не может быть пустым. Введите название задачи:", cancellationToken: cancellationToken);
                return;
            }

            if (!_taskData.ContainsKey(chatId)) {
                _taskData[chatId] = new Dictionary<string, object>();
            }
            _taskData[chatId]["Title"] = messageText;
            _taskStates[chatId] = "WaitingForComment"; 
            

            await botClient.SendTextMessageAsync(chatId, "Введите описание задачи:", cancellationToken: cancellationToken);
        }
        

        private async Task HandleCommentAsync(ITelegramBotClient botClient, long chatId, Update update, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(update.Message.Text)) {
                await botClient.SendTextMessageAsync(chatId, "Описание задачи не может быть пустым. Введите описание задачи:", cancellationToken: cancellationToken);
                return;
            }
            
            _taskData[chatId]["Comment"] = update.Message.Text;
            _taskStates[chatId] = "WaitingForAddress";
            
            await botClient.SendTextMessageAsync(chatId, "Введите адрес задачи:", cancellationToken: cancellationToken);
        }

        private async Task HandleAddressAsync(ITelegramBotClient botClient, long chatId, Update update, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(update.Message.Text)) {
                await botClient.SendTextMessageAsync(chatId, "Адрес задачи не может быть пустым. Введите адрес задачи:", cancellationToken: cancellationToken);
                return;
            }
            
            _taskData[chatId]["Address"] = update.Message.Text;
            _taskStates[chatId] = "WaitingForDeadline";
            
            await botClient.SendTextMessageAsync(chatId, "Введите дедлайн задачи (например, 2025-01-30):", cancellationToken: cancellationToken);
        }

        private async Task HandleDeadlineAsync(ITelegramBotClient botClient, long chatId, Update update, CancellationToken cancellationToken) {
            if (!DateTime.TryParse(update.Message.Text, out DateTime parsedDeadline)) {
                await botClient.SendTextMessageAsync(chatId, "Некорректный формат даты. Пожалуйста, введите дату в формате 'YYYY-MM-DD'.", cancellationToken: cancellationToken);
                return;
            }
            
            _taskData[chatId]["Deadline"] = parsedDeadline;
            await CreateTaskAsync(botClient, chatId, cancellationToken);
            _taskStates.Remove(chatId); 
            _taskData.Remove(chatId);  
        }

        private async Task CreateTaskAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken) {
            var title = _taskData[chatId]["Title"].ToString();
            var comment = _taskData[chatId]["Comment"].ToString();
            var address = _taskData[chatId]["Address"].ToString();
            var deadline = (DateTime)_taskData[chatId]["Deadline"];

            var allWorkers = await _dbContext.Users
                .ToListAsync(cancellationToken);

            var availableWorkers = allWorkers
                .Where(u => !_dbContext.Tasks.Any(t => t.WorkerId == u.Id && t.Deadline.Date == deadline.Date))
                .ToList();

            if (!availableWorkers.Any()) {
                await botClient.SendTextMessageAsync(chatId, "Нет доступных работников для этой даты.", cancellationToken: cancellationToken);
                return;
            }

            var randomWorker = availableWorkers[new Random().Next(availableWorkers.Count)];

            var task = new Model.Task() {
                Title = title,
                Deadline = deadline,
                Comment = comment,
                Address = address,
                CreatedAt = DateTime.UtcNow.AddHours(3),
                WorkerId = randomWorker.Id,
                Status = TaskStatus.Open
            };

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendTextMessageAsync(chatId, $"Задача '{title}' успешно создана!", cancellationToken: cancellationToken);
            _logger.LogInformation($"Задача '{title}' успешно создана для чата {chatId}");
        }
        
        
        public async Task SendTasksForTomorrowAsync(ITelegramBotClient botClient, long groupChatId, CancellationToken cancellationToken) {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);

            var tasks = await _dbContext.Tasks
                .Where(t => t.Deadline == tomorrow)
                .ToListAsync(cancellationToken);

            if (!tasks.Any()) {
                await botClient.SendTextMessageAsync(groupChatId, "На завтра задач нет.", cancellationToken: cancellationToken);
                return;
            }

            var message = string.Join("\n\n", tasks.Select(t => $"{t.Title}"));
            await botClient.SendTextMessageAsync(groupChatId, $"Задачи на завтра:\n{message}", cancellationToken: cancellationToken);
        }
        
        
        
    }
    
}
