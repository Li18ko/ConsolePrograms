using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Controller;
using TelegramBot.Helper;
using User = Telegram.Bots.Types.User;

namespace TelegramBot.Service;

public class UserStateService {
    private readonly ILogger<UserStateService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly TaskController _taskController;
    private readonly Dictionary<long, string> _userStates = new Dictionary<long, string>();

    public UserStateService(ILogger<UserStateService> logger, AppDbContext dbContext, TaskController taskController) {
        _dbContext = dbContext;
        _logger = logger;
        _taskController = taskController;
    }
    
    public async Task ProcessMessageAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken) {
        if (update.Message == null) return;
        var message = update.Message;
        
        if (!_userStates.ContainsKey(message.Chat.Id)) {
            _userStates[message.Chat.Id] = "start";
        }

        string userState = _userStates[message.Chat.Id];

        switch (userState) {
            case "start":
                await HandleStartState(botClient, update, cancellationToken);
                break;
            case "waiting_for_login":
                await HandleLoginState(botClient, update, cancellationToken);
                break;
            case var state when state.StartsWith("login:"):
                await HandlePasswordState(botClient, update, state, cancellationToken);
                break;
            case "waiting_for_task":
                if (update.Message.Text == "/task") {
                    await _taskController.HandleTaskCreationAsync(botClient, update, cancellationToken);
                }
                break;
            default:
                await SendMessageAsync(botClient, message.Chat.Id, $"Вы сказали: {message.Text}", cancellationToken);
                break;
        }
    }

    private async Task HandleStartState(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        if (update.Message.Text.ToLower() == "/start") {
            await SendMessageAsync(botClient, update.Message.Chat.Id, "Введите ваш логин:", cancellationToken);
            _userStates[update.Message.Chat.Id] = "waiting_for_login";
        }
    }
    
    private async Task HandleLoginState(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        _userStates[update.Message.Chat.Id] = $"login:{update.Message.Text}";
        await SendMessageAsync(botClient, update.Message.Chat.Id, "Введите ваш пароль:", cancellationToken);
    }

    private async Task HandlePasswordState(ITelegramBotClient botClient, Update update, string userState, 
        CancellationToken cancellationToken) {
        string login = userState.Split(':')[1];
        string password = update.Message.Text;
        
        _userStates[update.Message.Chat.Id] = "start";
        await botClient.DeleteMessageAsync(update.Message.Chat.Id, update.Message.MessageId, cancellationToken: cancellationToken);

        await SendMessageAsync(botClient, update.Message.Chat.Id, "Идет проверка данных...", cancellationToken);
        var user = await ValidateUserAsync(botClient, update.Message, login, password, cancellationToken);
        if (user == null) return;
            
        var role = await _dbContext.UserRoles
            .Where(ur => ur.UserId == user.Id && ur.RoleId == 3)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (role != null) {
            await SendMessageAsync(botClient, update.Message.Chat.Id, "Доступ есть!!", cancellationToken);
            _userStates[update.Message.Chat.Id] = "waiting_for_task";  
            await SendMessageAsync(botClient, update.Message.Chat.Id, "Вы можете начать создание задачи. Отправьте /task.", cancellationToken);

        } else {
            await SendMessageAsync(botClient, update.Message.Chat.Id, "Доступ ограничен!!", cancellationToken);
        }
    }
    
    private async Task SendMessageAsync(ITelegramBotClient botClient, long chatId, string text, CancellationToken cancellationToken) {
        await botClient.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken);
    }
    
    private async Task<Model.User?> ValidateUserAsync(ITelegramBotClient botClient, Message message, string login, string password, CancellationToken cancellationToken) {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login, cancellationToken: cancellationToken);
        if (user == null || !PasswordHelper.VerifyPassword(user.Password, PasswordHelper.HashPassword(password))) {
            await SendMessageAsync(botClient, message.Chat.Id, "Неверный логин или пароль.", cancellationToken);
            return null;
        }

        return user;
    }

}