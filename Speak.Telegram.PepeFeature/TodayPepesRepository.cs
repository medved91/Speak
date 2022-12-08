using System.Collections.Concurrent;
using System.Linq.Expressions;
using Speak.Telegram.CommonContracts;
using Speak.Telegram.PepeFeature.Entities;

namespace Speak.Telegram.PepeFeature;

internal class TodayPepesRepository : IRepository<TodayPepe>
{
    private readonly ConcurrentBag<TodayPepe> _todayPepes = new();

    public Task<TodayPepe> AddAsync(TodayPepe player, CancellationToken ct)
    {
        _todayPepes.Add(player);
        
        return Task.FromResult(player);
    }

    public Task<TodayPepe?> FirstOrDefaultAsync(Expression<Func<TodayPepe, bool>> predicate, CancellationToken ct)
    {
        var firstOrDefault = _todayPepes.FirstOrDefault(predicate.Compile());

        return Task.FromResult(firstOrDefault);
    }
}