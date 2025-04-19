using Zoo.Application.Abstractions;
using Zoo.Domain.Entities;
using System.Linq;

namespace Zoo.Application.Statistics
{
    /// <summary>
    /// Реализация подсчета общего числа животных и свободных мест.
    /// </summary>
    public sealed class ZooStatisticsService : IZooStatisticsService
    {
        private readonly IRepository<Animal> _animals;
        private readonly IRepository<Enclosure> _enclosures;

        public ZooStatisticsService(
            IRepository<Animal> animals,
            IRepository<Enclosure> enclosures)
        {
            _animals = animals;
            _enclosures = enclosures;
        }

        public async ValueTask<ZooStatsDto> GetAsync(CancellationToken ct = default)
        {
            var allAnimals = await _animals.GetAllAsync(ct);
            var allEncls = await _enclosures.GetAllAsync(ct);

            int predFree = allEncls.Where(e => e.Type == "Predator")
                                    .Sum(e => e.Capacity - e.AnimalIds.Count);
            int herbFree = allEncls.Where(e => e.Type == "Herbivore")
                                    .Sum(e => e.Capacity - e.AnimalIds.Count);

            return new ZooStatsDto(allAnimals.Count(), predFree, herbFree);
        }
    }
}
