using System.Collections.Concurrent;
using Speak.Storage;
using Speak.Telegram.Bot.Entities;

namespace Speak.Telegram.Bot.Storage;

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