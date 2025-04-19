using Microsoft.AspNetCore.Mvc;
using Zoo.Application.Abstractions;
using Zoo.Domain.Entities;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/enclosures")]
    public class EnclosuresController : ControllerBase
    {
        private readonly IRepository<Enclosure> _encls;

        public EnclosuresController(IRepository<Enclosure> encls)
            => _encls = encls;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _encls.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var enc = await _encls.GetAsync(id);
            if (enc is null) return NotFound();
            return Ok(enc);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEnclosureDto dto)
        {
            var enc = new Enclosure(dto.Type, dto.Size, dto.Capacity);
            await _encls.AddAsync(enc);
            await _encls.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = enc.Id }, enc);
        }
    }

    public record CreateEnclosureDto(string Type, int Size, int Capacity);
}
