using Microsoft.AspNetCore.Mvc;
using Speak.TelegramBot;
using Telegram.Bot.Types;

namespace Speak.Web.Controllers;

public class TelegramWebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromServices] ITelegramMessageRouter handleUpdateService,
        [FromBody] Update update)
    {
        await handleUpdateService.HandleNewMessageAsync(update);
        return Ok();
    }
}
