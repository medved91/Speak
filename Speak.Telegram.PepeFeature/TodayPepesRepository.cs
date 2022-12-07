using System.Collections.Concurrent;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.PepeFeature.Entities;

namespace Speak.Telegram.PepeFeature;

internal class TodayPepesRepository : IRepository<TodayPepe>
{
    private readonly ConcurrentBag<TodayPepe> _todayPepes = new();

    public Task AddAsync(TodayPepe pepe, CancellationToken ct)
    {
        _todayPepes.Add(pepe);
        
        return Task.CompletedTask;
    }

    public Task<TodayPepe?> FirstOrDefaultAsync(Func<TodayPepe, bool> predicate, CancellationToken ct)
    {
        var firstOrDefault = _todayPepes.FirstOrDefault(predicate);

        return Task.FromResult(firstOrDefault);
    }
}