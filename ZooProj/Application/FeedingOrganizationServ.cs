using Zoo.Application.Abstractions;
using Zoo.Domain.Entities;

namespace Zoo.Application.Feeding
{
    /// <summary>
    /// Создаёт новые расписания и помечает их выполненными.
    /// </summary>
    public sealed class FeedingOrganizationService : IFeedingOrganizationService
    {
        private readonly IRepository<Animal> _animals;
        private readonly IRepository<FeedingSchedule> _schedules;

        public FeedingOrganizationService(
            IRepository<Animal> animals,
            IRepository<FeedingSchedule> schedules)
        {
            _animals = animals;
            _schedules = schedules;
        }

        public async ValueTask<FeedingSchedule> AddScheduleAsync(Guid animalId, TimeOnly time, CancellationToken ct = default)
        {
            var animal = await _animals.GetAsync(animalId, ct)
                         ?? throw new KeyNotFoundException("Животное не найдено");
            var schedule = new FeedingSchedule(animal.Id, time, animal.FavouriteFood);
            await _schedules.AddAsync(schedule, ct);
            await _schedules.SaveChangesAsync(ct);
            return schedule;
        }

        public async ValueTask MarkCompletedAsync(Guid scheduleId, CancellationToken ct = default)
        {
            var sched = await _schedules.GetAsync(scheduleId, ct)
                       ?? throw new KeyNotFoundException("Расписание не найдено");
            sched.MarkCompleted();
            await _schedules.SaveChangesAsync(ct);
        }
    }
}
