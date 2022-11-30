namespace Speak.Storage;

public interface IRepository<TEntity>
{
    Task AddAsync(TEntity entity);

    public Task<TEntity?> FirstOrDefaultAsync(Func<TEntity, bool> predicate);
}