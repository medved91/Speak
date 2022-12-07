using Speak.Telegram.CommonContracts;
using Speak.Telegram.CutieFeature.Contracts;

namespace Speak.Telegram.CutieFeature;

internal interface ICutieRepository
{
    public IRepository<CutiePlayer> Players { get; }
}