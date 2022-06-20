using Microsoft.AspNetCore.SignalR;

namespace Speak.Web.Hubs;

public class WebRtcHub : Hub
{
    private readonly ILogger<WebRtcHub> _logger;
    
    public WebRtcHub(ILogger<WebRtcHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Метод получения идентификатора подключения текущего пользователя
    /// </summary>
    public string GetConnectionId() => Context.ConnectionId;

    /// <summary>
    /// Метод, который вызывается, чтобы сохранить имя текущего пользователя
    /// </summary>
    public void SetMyName(string name)
    {
        var me = Users.ConnectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
        if (me != null) me.Name = name;
    }

    /// <summary>
    /// Метод вызывается при создании подключения с пользователем на фронте, чтоб узнать его имя
    /// </summary>
    public string GetUserName(string userConnectionId) => 
        Users.ConnectedUsers.FirstOrDefault(u => u.ConnectionId == userConnectionId)?.Name ?? "Без имени";

    /// <summary>
    /// Метод получения идентификаторов всех подключенных пользователей, кроме текущего
    /// </summary>
    public string[] GetOtherConnectedUsers() => 
        Users.ConnectedUsers
            .Where(i => i.ConnectionId != Context.ConnectionId)
            .Select(u => u.ConnectionId)
            .ToArray();

    /// <summary>
    /// Метод, который вызывается клиентом для отправки оффера другому пользователю.
    /// По сути своей метод просто проксирует оффер от одного клиента другому, вызывая на стороне
    /// принимающего пользователя фронтовый метод ReceiveOffer
    /// </summary>
    /// <param name="toUserId">Идентификатор подключения пользователя, которому высылается оффер</param>
    /// <param name="offerDescription">Сериализованный оффер</param>
    public async Task SendOffer(string toUserId, string offerDescription)
    {
        var fromUserId = Context.ConnectionId;
        
        _logger.LogInformation("Отправляем оффер от {Sender} клиенту {Reciever}", fromUserId, toUserId);
        
        await Clients.Client(toUserId).SendAsync("ReceiveOffer", offerDescription, fromUserId);
    }

    /// <summary>
    /// Метод вызывается клиентом для отправки ответа на оффер.
    /// Проксирует ансвер от клиента другому клиенту, вызывая на фронте ReceiveAnswer
    /// </summary>
    /// <param name="toUserId">Клиент, которому будет отправлен ответ</param>
    /// <param name="answerDescription">Сериализованный answer</param>
    public async Task SendAnswerToUser(string toUserId, string answerDescription)
    {
        var fromUserId = Context.ConnectionId;
        
        _logger.LogInformation("Отправляем ответ от {Sender} клиенту {Reciever}", fromUserId, toUserId);

        await Clients.Client(toUserId).SendAsync("ReceiveAnswer", answerDescription, fromUserId);
    }

    /// <summary>
    /// Метод вызывается клиентом для обмена ICE-кандидатами
    /// </summary>
    /// <param name="toUserId">Идентификатор подключения пользователя, которому будет выслан ICE-кандидат</param>
    /// <param name="iceCandidate">Сериализованный ICE-кандидат</param>
    public async Task SendIceCandidateToOtherPeer(string toUserId, string iceCandidate)
    {
        var fromUserId = Context.ConnectionId;

        _logger.LogInformation("Отсылаем Ice-candidate от {Sender} кленту {Reciever}", fromUserId, toUserId);

        await Clients.Client(toUserId).SendAsync("ReceiveIceCandidate", iceCandidate, fromUserId);
    }
    
    /// <summary>
    /// Метод вызывается фронтом нового пользователя для оповещения остальных о его пришествии.
    /// Остальные, получив уведомление, вышлют оффер новому клиенту
    /// </summary>
    public async Task NotifyOthersAboutNewUser()
    {
        await Clients.Others.SendAsync("SendOfferToUser", Context.ConnectionId);
    }

    public override Task OnConnectedAsync()
    {
        Users.ConnectedUsers.Add(new User(Context.ConnectionId));

        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userToRemove = Users.ConnectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
        if (userToRemove != null) Users.ConnectedUsers.Remove(userToRemove);

        await Clients.Others.SendAsync("DisconnectUser", Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }
}