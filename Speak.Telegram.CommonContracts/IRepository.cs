using System.Linq.Expressions;

namespace Speak.Telegram.CommonContracts;

public interface IRepository<TEntity>
{
    public Task<TEntity> AddAsync(TEntity player, CancellationToken ct);

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);
}