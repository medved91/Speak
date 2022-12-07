namespace Speak.Telegram.CommonContracts;

public interface IRepository<TEntity>
{
    public Task AddAsync(TEntity pepe, CancellationToken ct);

    public Task<TEntity?> FirstOrDefaultAsync(Func<TEntity, bool> predicate, CancellationToken ct);
}