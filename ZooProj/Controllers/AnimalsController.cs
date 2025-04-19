using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using Zoo.Application.Abstractions;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IRepository<Animal> _animals;

        public AnimalsController(IRepository<Animal> animals)
            => _animals = animals;

        // GET api/animals
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _animals.GetAllAsync());

        // GET api/animals/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var animal = await _animals.GetAsync(id);
            if (animal is null) return NotFound();
            return Ok(animal);
        }

        // POST api/animals
        [HttpPost]
        public async Task<IActionResult> Create(CreateAnimalDto dto)
        {
            var species = Species.Create(dto.Species, dto.IsPredator);
            var food = new FoodType(dto.FavouriteFood);
            var animal = new Animal(species, dto.Nickname, dto.BirthDate, dto.Sex, food);

            await _animals.AddAsync(animal);
            await _animals.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = animal.Id }, animal);
        }

        // DELETE api/animals/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _animals.RemoveAsync(id);
            await _animals.SaveChangesAsync();
            return NoContent();
        }
    }

    public record CreateAnimalDto(
        string Species,
        bool IsPredator,
        string Nickname,
        DateOnly BirthDate,
        char Sex,
        string FavouriteFood
    );
}
