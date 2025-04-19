using Zoo.Domain.Entities;

namespace Zoo.Application.Feeding
{
    /// <summary>
    /// Сервис управления расписанием кормления.
    /// </summary>
    public interface IFeedingOrganizationService
    {
        ValueTask<FeedingSchedule> AddScheduleAsync(Guid animalId, TimeOnly time, CancellationToken ct = default);
        ValueTask MarkCompletedAsync(Guid scheduleId, CancellationToken ct = default);
    }
}
