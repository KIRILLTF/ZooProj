using Zoo.Domain.Primitives;

namespace Zoo.Domain.ValueObjects
{
    /// <summary>
    /// Тип корма.
    /// </summary>
    public sealed record FoodType(string Name) : ValueObject;
}
