namespace Speak.Telegram.Bot.FeatureHandlers;

/// <summary>
/// Контракт хендлера фичи
/// </summary>
/// <typeparam name="TFeatureRequest">Тип реквеста на выполнение фичи</typeparam>
public interface ITelegramFeatureHandler<in TFeatureRequest>
{
    /// <summary>
    /// Выполнить фичу
    /// </summary>
    Task Handle(TFeatureRequest request);
}

/// <summary>
/// Контракт хендлера фичи, возвращающего результат
/// </summary>
/// <typeparam name="TFeatureRequest">Тип реквеста на выполнение фичи</typeparam>
/// <typeparam name="TResult">Тип результата выполнения фичи</typeparam>
public interface ITelegramFeatureHandler<in TFeatureRequest, TResult>
{
    /// <summary>
    /// Выполнить фичу
    /// </summary>
    Task<TResult> Handle(TFeatureRequest request);
}