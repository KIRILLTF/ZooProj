using Zoo.Application.Abstractions;

namespace Zoo.Infrastructure.Persistence
{
    /// <summary>
    /// Простое потокобезопасное in‑memory хранилище для демонстрации.
    /// </summary>
    public sealed class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _store = new();
        private readonly Func<T, Guid> _getId;

        public InMemoryRepository(Func<T, Guid> getId) => _getId = getId;

        public ValueTask AddAsync(T entity, CancellationToken ct = default)
        {
            _store[_getId(entity)] = entity;
            return ValueTask.CompletedTask;
        }

        public ValueTask<T?> GetAsync(Guid id, CancellationToken ct = default)
            => ValueTask.FromResult(_store.TryGetValue(id, out var e) ? e : null);

        public ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
            => ValueTask.FromResult<IEnumerable<T>>(_store.Values.ToList());

        public ValueTask RemoveAsync(Guid id, CancellationToken ct = default)
        {
            _store.Remove(id);
            return ValueTask.CompletedTask;
        }

        public ValueTask SaveChangesAsync(CancellationToken ct = default)
            => ValueTask.CompletedTask;
    }
}
