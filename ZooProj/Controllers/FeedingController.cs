using Microsoft.AspNetCore.Mvc;
using Zoo.Application.Feeding;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/feeding")]
    public class FeedingController : ControllerBase
    {
        private readonly IFeedingOrganizationService _feeding;

        public FeedingController(IFeedingOrganizationService feeding)
            => _feeding = feeding;

        // POST api/feeding
        [HttpPost]
        public async Task<IActionResult> Add(AddScheduleDto dto)
            => Ok(await _feeding.AddScheduleAsync(dto.AnimalId, dto.Time));

        // POST api/feeding/{scheduleId}/complete
        [HttpPost("{scheduleId:guid}/complete")]
        public async Task<IActionResult> Complete(Guid scheduleId)
        {
            await _feeding.MarkCompletedAsync(scheduleId);
            return NoContent();
        }
    }

    public record AddScheduleDto(Guid AnimalId, TimeOnly Time);
}
