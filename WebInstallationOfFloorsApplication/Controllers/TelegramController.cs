using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebInstallationOfFloorsApplication;

[ApiController]
[Route("api/[controller]")]
public class TelegramController: ControllerBase {
    private readonly TelegramService _telegramService;

    public TelegramController(TelegramService telegramService) {
        _telegramService = telegramService;
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("sendMessage")]
    public async System.Threading.Tasks.Task SendMessageAsync(CancellationToken cancellationToken) {
        await _telegramService.SendDailyMessagesAsync(cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async System.Threading.Tasks.Task HandleWebhookAsync([FromBody] CallbackQueryDto callbackQueryDto, 
        CancellationToken cancellationToken) {
        await _telegramService.HandleWebhookUpdateAsync(callbackQueryDto, cancellationToken);
    }
}