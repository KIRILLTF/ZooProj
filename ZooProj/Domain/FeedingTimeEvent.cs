using Zoo.Domain.Events;

namespace Zoo.Domain.Events
{
    /// <summary>
    /// Событие: наступило время кормления, помечаем выполнение.
    /// </summary>
    public sealed record FeedingTimeEvent(
        Guid ScheduleId,
        Guid AnimalId,
        DateTime OccurredOn
    ) : IDomainEvent;
}
