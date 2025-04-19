using Zoo.Application.Abstractions;
using Zoo.Domain.Entities;

namespace Zoo.Application.Animals
{
    /// <summary>
    /// Реализация логики перемещения с валидацией вместимости и типа вольера.
    /// </summary>
    public sealed class AnimalTransferService : IAnimalTransferService
    {
        private readonly IRepository<Animal> _animals;
        private readonly IRepository<Enclosure> _enclosures;

        public AnimalTransferService(
            IRepository<Animal> animals,
            IRepository<Enclosure> enclosures)
        {
            _animals = animals;
            _enclosures = enclosures;
        }

        public async ValueTask MoveAsync(Guid animalId, Guid toEnclosureId, CancellationToken ct = default)
        {
            var animal = await _animals.GetAsync(animalId, ct)
                         ?? throw new KeyNotFoundException("Животное не найдено");
            var toEnc = await _enclosures.GetAsync(toEnclosureId, ct)
                         ?? throw new KeyNotFoundException("Вольер не найден");
            var fromEnc = await _enclosures.GetAsync(animal.EnclosureId, ct);

            if (!toEnc.HasSpace)
                throw new InvalidOperationException("Целевой вольер полон");

            // Предотвращаем опасных соседей:
            if (animal.Species.IsPredator && toEnc.Type != "Predator" ||
                !animal.Species.IsPredator && toEnc.Type == "Predator")
                throw new InvalidOperationException("Неподходящий тип вольера");

            fromEnc?.Remove(animal.Id);
            toEnc.Add(animal.Id);
            animal.MoveTo(toEnc.Id);

            await _enclosures.SaveChangesAsync(ct);
            await _animals.SaveChangesAsync(ct);
        }
    }
}
