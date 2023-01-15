using System.Text.RegularExpressions;
using Speak.Telegram.ChatMigrationFeatureContracts;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts.Requests;
using Speak.Telegram.MusicQuizFeatureContracts;
using Speak.Telegram.PepeFeature;
using Speak.Telegram.Postgres;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Speak.Telegram.Bot;

internal class MessageHandlerFactory : IMessageHandlerFactory
{
    private readonly ITelegramFeatureHandler<PickWhichPepeAmITodayFeatureRequest, Message> _pepePickerHandler;
    private readonly ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message> _regInCutieHandler;
    private readonly ITelegramFeatureHandler<StartCutieElectionsFeatureRequest, Message> _startCutieElectionsHandler;
    private readonly ITelegramFeatureHandler<SendMissionResultFeatureRequest, Message> _missionResultHandler;
    private readonly ITelegramFeatureHandler<MigrateChatFeatureRequest, Message> _chatMigrationHandler;
    private readonly ITelegramFeatureHandler<StartMusicQuizFeatureRequest, Message> _startMusicQuizHandler;
    private readonly ITelegramFeatureHandler<SendMusicQuizAnswerFeatureRequest, Message> _sendMusicQuizResultsHandler;

    public MessageHandlerFactory(
        ITelegramFeatureHandler<PickWhichPepeAmITodayFeatureRequest, Message> pepePickerHandler, 
        ITelegramFeatureHandler<StartCutieElectionsFeatureRequest, Message> startCutieElectionsHandler, 
        ITelegramFeatureHandler<RegisterInCutieFeatureRequest, Message> regInCutieHandler, 
        ITelegramFeatureHandler<SendMissionResultFeatureRequest, Message> missionResultHandler, 
        ITelegramFeatureHandler<MigrateChatFeatureRequest, Message> chatMigrationHandler, 
        ITelegramFeatureHandler<StartMusicQuizFeatureRequest, Message> startMusicQuizHandler, 
        ITelegramFeatureHandler<SendMusicQuizAnswerFeatureRequest, Message> sendMusicQuizResultsHandler)
    {
        _startCutieElectionsHandler = startCutieElectionsHandler;
        _regInCutieHandler = regInCutieHandler;
        _missionResultHandler = missionResultHandler;
        _chatMigrationHandler = chatMigrationHandler;
        _pepePickerHandler = pepePickerHandler;
        _startMusicQuizHandler = startMusicQuizHandler;
        _sendMusicQuizResultsHandler = sendMusicQuizResultsHandler;
    }
    
    public Task<Message>? GetHandlerFor(Message message, CancellationToken ct)
    {
        var action = message.Type switch
        {
            MessageType.Text => GetTextMessageHandler(message, ct),
            MessageType.Photo => GetPhotoMessageHandler(message, ct),
            MessageType.MigratedFromGroup => GetChatMigrationHandler(message, ct),
            MessageType.MigratedToSupergroup => GetChatMigrationHandler(message, ct),
            _ => null
        };

        return action;
    }

    public Task<Message>? GetHandlerFor(CallbackQuery callback, CancellationToken ct)
    {
        var result = callback.Data?.ToLower() switch
        {
            "wrong" => AnswerCallback.Wrong,
            "correct" => AnswerCallback.Correct,
            _ => AnswerCallback.Unknown
        };
        
        var request = new SendMusicQuizAnswerFeatureRequest(callback.Message?.Chat.Id, callback.Id, 
            callback.From.Username, callback.Message?.MessageId, result);
        
        return _sendMusicQuizResultsHandler.Handle(request, ct);
    }

    private Task<Message>? GetTextMessageHandler(Message message, CancellationToken ct)
    {
        var action = message.Text!.Split(' ')[0] switch
        {
            var pepe when Regex.IsMatch(pepe, @"^\/pepe[@]?")
                => _pepePickerHandler.Handle(new PickWhichPepeAmITodayFeatureRequest(
                    message.From?.Username, message.Chat.Id, message.MessageId), ct),

            var registerInCutie when Regex.IsMatch(registerInCutie, @"^\/join_cutie[@]?")
                => _regInCutieHandler.Handle(new RegisterInCutieFeatureRequest(message.From?.Username,
                    message.From?.FirstName, message.From?.LastName, message.Chat.Id, message.MessageId), ct),

            var cutieElections when Regex.IsMatch(cutieElections, @"^\/get_cutie[@]?")
                => _startCutieElectionsHandler.Handle(new StartCutieElectionsFeatureRequest(message.Chat.Id), ct),
            
            var musicQuiz when Regex.IsMatch(musicQuiz, @"^\/quiz_me[@]?")
                => _startMusicQuizHandler.Handle(new StartMusicQuizFeatureRequest(message.Chat.Id, message.From?.Username), ct),

            _ => null
        };
        
        return action;
    }

    private Task<Message>? GetPhotoMessageHandler(Message message, CancellationToken ct)
    {
        // Сейчас фотку могут прислать только в ответ на задание Лапусечки
        // Если фотка пришла не в ответ на сообщение - игнорируем
        if (message.ReplyToMessage == null) return null;

        return _missionResultHandler.Handle(new SendMissionResultFeatureRequest(message.Chat.Id, 
            message.ReplyToMessage.MessageId,
            message.From?.Username,
            message.MessageId), ct);
    }

    private Task<Message> GetChatMigrationHandler(Message message, CancellationToken ct)
    {
        return _chatMigrationHandler.Handle(
            new MigrateChatFeatureRequest(message.Chat.Id, message.MigrateToChatId), ct);
    }
}