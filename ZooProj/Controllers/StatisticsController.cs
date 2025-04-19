using Microsoft.AspNetCore.Mvc;
using Zoo.Application.Statistics;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatisticsController : ControllerBase
    {
        private readonly IZooStatisticsService _stats;

        public StatisticsController(IZooStatisticsService stats)
            => _stats = stats;

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _stats.GetAsync());
    }
}
