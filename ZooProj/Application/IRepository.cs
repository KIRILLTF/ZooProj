namespace Zoo.Application.Abstractions
{
    /// <summary>
    /// Общий интерфейс репозитория для CRUD-операций.
    /// </summary>
    public interface IRepository<T>
    {
        ValueTask<T?> GetAsync(Guid id, CancellationToken ct = default);
        ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        ValueTask AddAsync(T entity, CancellationToken ct = default);
        ValueTask RemoveAsync(Guid id, CancellationToken ct = default);
        ValueTask SaveChangesAsync(CancellationToken ct = default);
    }
}
