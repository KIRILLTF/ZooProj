using Zoo.Domain.Events;

namespace Zoo.Domain.Events
{
    /// <summary>
    /// Событие: животное переместили из одного вольера в другой.
    /// </summary>
    public sealed record AnimalMovedEvent(
        Guid AnimalId,
        Guid FromEnclosureId,
        Guid ToEnclosureId,
        DateTime OccurredOn
    ) : IDomainEvent;
}
