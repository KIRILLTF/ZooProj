using Zoo.Domain.Events;
using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
    /// <summary>
    /// Доменный объект «Животное».
    /// Инкапсулирует правила кормления, лечения и перемещения.
    /// </summary>
    public class Animal
    {
        private readonly List<IDomainEvent> _events = new();

        public Guid Id { get; init; } = Guid.NewGuid();
        public Species Species { get; init; }
        public string Nickname { get; init; }
        public DateOnly BirthDate { get; init; }
        public char Sex { get; init; }  // 'M' или 'F'
        public FoodType FavouriteFood { get; init; }
        public bool IsHealthy { get; private set; } = true;
        public Guid EnclosureId { get; private set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

        public Animal(Species species,
                      string nickname,
                      DateOnly birthDate,
                      char sex,
                      FoodType favouriteFood)
        {
            Species = species;
            Nickname = nickname;
            BirthDate = birthDate;
            Sex = sex;
            FavouriteFood = favouriteFood;
        }

        /// <summary>Кормим животное.</summary>
        public void Feed() { /* … */ }

        /// <summary>Ставит животное в здоровый статус.</summary>
        public void Heal() => IsHealthy = true;

        /// <summary>
        /// Перемещает животное в новый вольер
        /// и генерирует событие AnimalMovedEvent.
        /// </summary>
        public void MoveTo(Guid newEnclosureId)
        {
            var oldId = EnclosureId;
            EnclosureId = newEnclosureId;
            _events.Add(new AnimalMovedEvent(Id, oldId, newEnclosureId, DateTime.UtcNow));
        }

        public void ClearEvents() => _events.Clear();
    }
}
