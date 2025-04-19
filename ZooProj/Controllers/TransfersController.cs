using Microsoft.AspNetCore.Mvc;
using Zoo.Application.Animals;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    public class TransfersController : ControllerBase
    {
        private readonly IAnimalTransferService _transfer;

        public TransfersController(IAnimalTransferService transfer)
            => _transfer = transfer;

        // POST api/transfers
        [HttpPost]
        public async Task<IActionResult> Move(MoveDto dto)
        {
            await _transfer.MoveAsync(dto.AnimalId, dto.ToEnclosureId);
            return NoContent();
        }
    }

    public record MoveDto(Guid AnimalId, Guid ToEnclosureId);
}
