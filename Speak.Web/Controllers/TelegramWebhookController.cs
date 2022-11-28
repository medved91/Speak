using Microsoft.AspNetCore.Mvc;
using Speak.TelegramBot;
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
    public async Task<IActionResult> Post([FromServices] ITelegramMessageRouter handleUpdateService,
        [FromBody] Update update)
    {
        //TODO: Прикрутить серилог, а то @ не работает
        _logger.LogInformation("Получено сообщение боту Telegram: {@MessageBody}", update);
        
        await handleUpdateService.HandleNewMessageAsync(update);
        return Ok();
    }
}
