using System.Collections.Concurrent;
using Speak.Storage;
using Speak.TelegramBot.Entities;

namespace Speak.TelegramBot.Storage;

internal class TodayPepesRepository : IRepository<TodayPepe>
{
    private readonly ConcurrentBag<TodayPepe> _todayPepes = new();

    public Task AddAsync(TodayPepe pepe)
    {
        _todayPepes.Add(pepe);
        
        return Task.CompletedTask;
    }

    public Task<TodayPepe?> FirstOrDefaultAsync(Func<TodayPepe, bool> predicate)
    {
        var firstOrDefault = _todayPepes.FirstOrDefault(predicate);

        return Task.FromResult(firstOrDefault);
    }
}