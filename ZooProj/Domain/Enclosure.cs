namespace Zoo.Domain.Entities
{
    /// <summary>
    /// Вольер: хранит список животных, умеет добавлять/удалять и проверять вместимость.
    /// </summary>
    public class Enclosure
    {
        private readonly List<Guid> _animalIds = new();

        public Guid Id { get; init; } = Guid.NewGuid();
        public string Type { get; init; }      // "Predator", "Herbivore" и т.п.
        public int Size { get; init; }         // условная площадь
        public int Capacity { get; init; }     // макс. кол-во животных

        public IReadOnlyCollection<Guid> AnimalIds => _animalIds.AsReadOnly();
        public bool HasSpace => _animalIds.Count < Capacity;

        public Enclosure(string type, int size, int capacity)
            => (Type, Size, Capacity) = (type, size, capacity);

        public void Add(Guid animalId)
        {
            if (!HasSpace)
                throw new InvalidOperationException("Вольер полон");
            _animalIds.Add(animalId);
        }

        public void Remove(Guid animalId) => _animalIds.Remove(animalId);

        public void Clean() { /* уборка */ }
    }
}
