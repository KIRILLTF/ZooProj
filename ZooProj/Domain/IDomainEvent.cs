namespace Zoo.Domain.Events
{
    /// <summary>
    /// Общий интерфейс для доменных событий.
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
