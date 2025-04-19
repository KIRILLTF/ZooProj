using Zoo.Domain.Primitives;

namespace Zoo.Domain.ValueObjects
{
    /// <summary>
    /// Вид животного и признак, является ли животное хищником.
    /// </summary>
    public sealed record Species
    {
        public string Name { get; init; }
        public bool IsPredator { get; init; }

        private Species(string name, bool isPredator)
            => (Name, IsPredator) = (name, isPredator);

        public static Species Create(string name, bool isPredator)
            => new(name, isPredator);
    }
}
