using Zoo.Domain.Events;
using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
    /// <summary>
    /// Расписание кормления одного животного.
    /// Генерирует событие при отметке о выполнении.
    /// </summary>
    public class FeedingSchedule
    {
        private readonly List<IDomainEvent> _events = new();

        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid AnimalId { get; init; }
        public TimeOnly Time { get; private set; }
        public FoodType Food { get; private set; }
        public bool Completed { get; private set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

        public FeedingSchedule(Guid animalId, TimeOnly time, FoodType food)
            => (AnimalId, Time, Food) = (animalId, time, food);

        /// <summary>Переназначить время кормления.</summary>
        public void Reschedule(TimeOnly newTime) => Time = newTime;

        /// <summary>Отметить, что кормление выполнено.</summary>
        public void MarkCompleted()
        {
            Completed = true;
            _events.Add(new FeedingTimeEvent(Id, AnimalId, DateTime.UtcNow));
        }

        public void ClearEvents() => _events.Clear();
    }
}
