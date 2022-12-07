using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.CutieFeature;

public class CutiePlayersRepository : IRepository<CutiePlayer>
{
    public async Task AddAsync(CutiePlayer pepe, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<CutiePlayer?> FirstOrDefaultAsync(Func<CutiePlayer, bool> predicate, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}