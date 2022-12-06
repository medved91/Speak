using Microsoft.AspNetCore.Mvc;
using Speak.Telegram.Bot;
using Telegram.Bot.Types;

namespace Speak.Web.Controllers;

public class TelegramWebhookController : ControllerBase
{
    private readonly ILogger<TelegramWebhookController> _logger;

    public TelegramWebhookController(ILogger<TelegramWebhookController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromServices] ITelegramMessageRouter messageRouter,
        [FromBody] Update update)
    {
        _logger.LogInformation("Получено сообщение из Telegram: {@MessageBody}", update);
        
        await messageRouter.HandleNewMessageAsync(update);
        return Ok();
    }
}
