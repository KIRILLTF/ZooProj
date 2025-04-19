using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Zoo.Presentation.Controllers;
using Zoo.Application.Abstractions;
using Zoo.Application.Animals;
using Zoo.Application.Feeding;
using Zoo.Application.Statistics;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Persistence;

namespace ZooTests.Controllers
{
    /// <summary>
    /// Проверяем работу AnimalsController:
    /// - создание животного
    /// - получение списка животных
    /// </summary>
    public class AnimalsControllerTests
    {
        [Fact]
        public async Task CreateAndGetAnimal_Works()
        {
            var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
            var controller = new AnimalsController(animalRepo);

            // DTO конструируется позиционно, без именованных параметров
            var dto = new CreateAnimalDto(
                "Elephant",   // species
                false,        // isPredator
                "Elly",       // nickname
                DateOnly.FromDateTime(DateTime.Today),
                'F',          // sex
                "Grass"       // favouriteFood
            );

            var createResult = await controller.Create(dto);
            var createdAt = Assert.IsType<CreatedAtActionResult>(createResult);
            var created = Assert.IsType<Animal>(createdAt.Value);
            Assert.Equal("Elly", created.Nickname);

            var getAllResult = await controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(getAllResult);
            var list = Assert.IsAssignableFrom<IEnumerable<Animal>>(okResult.Value);
            Assert.Contains(list, a => a.Id == created.Id);
        }
    }

    /// <summary>
    /// Проверяем работу EnclosuresController:
    /// - создание вольера
    /// - получение списка вольеров
    /// </summary>
    public class EnclosuresControllerTests
    {
        [Fact]
        public async Task CreateAndGetEnclosure_Works()
        {
            var repo = new InMemoryRepository<Enclosure>(e => e.Id);
            var controller = new EnclosuresController(repo);

            var dto = new CreateEnclosureDto(
                "Herbivore", // type
                150,         // size
                5            // capacity
            );

            var createResult = await controller.Create(dto);
            var createdAt = Assert.IsType<CreatedAtActionResult>(createResult);
            var created = Assert.IsType<Enclosure>(createdAt.Value);
            Assert.Equal("Herbivore", created.Type);

            var getAllResult = await controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(getAllResult);
            var list = Assert.IsAssignableFrom<IEnumerable<Enclosure>>(okResult.Value);
            Assert.Contains(list, e => e.Id == created.Id);
        }
    }

    /// <summary>
    /// Проверяем работу FeedingController:
    /// - добавление и завершение кормления
    /// </summary>
    public class FeedingControllerTests
    {
        [Fact]
        public async Task AddAndCompleteFeeding_Works()
        {
            var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
            var scheduleRepo = new InMemoryRepository<FeedingSchedule>(f => f.Id);

            var species = Species.Create("Bear", true);
            var food = new FoodType("Berries");
            var animal = new Animal(
                species,
                "Baloo",
                DateOnly.FromDateTime(DateTime.Today),
                'M',
                food
            );
            await animalRepo.AddAsync(animal);

            var service = new FeedingOrganizationService(animalRepo, scheduleRepo);
            var controller = new FeedingController(service);

            // создаём расписание
            var dtoAdd = new AddScheduleDto(animal.Id, new TimeOnly(10, 0));
            var addRes = await controller.Add(dtoAdd);
            var okAdd = Assert.IsType<OkObjectResult>(addRes);
            var sched = Assert.IsType<FeedingSchedule>(okAdd.Value);
            Assert.Equal(animal.Id, sched.AnimalId);

            // отмечаем кормление
            var completeRes = await controller.Complete(sched.Id);
            Assert.IsType<NoContentResult>(completeRes);

            var updated = await scheduleRepo.GetAsync(sched.Id);
            Assert.True(updated.Completed);
        }
    }

    /// <summary>
    /// Проверяем работу TransfersController:
    /// - перемещение животного между вольерами
    /// </summary>
    public class TransfersControllerTests
    {
        [Fact]
        public async Task MoveAnimal_Works()
        {
            var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
            var enclRepo = new InMemoryRepository<Enclosure>(e => e.Id);

            var species = Species.Create("Lion", true);
            var food = new FoodType("Meat");
            var animal = new Animal(
                species,
                "Leo",
                DateOnly.FromDateTime(DateTime.Today),
                'M',
                food
            );
            var src = new Enclosure("Predator", 100, 2);
            src.Add(animal.Id);
            animal.MoveTo(src.Id);

            await animalRepo.AddAsync(animal);
            await enclRepo.AddAsync(src);

            var dst = new Enclosure("Predator", 50, 2);
            await enclRepo.AddAsync(dst);

            var service = new AnimalTransferService(animalRepo, enclRepo);
            var controller = new TransfersController(service);

            var dtoMove = new MoveDto(animal.Id, dst.Id);
            var resMove = await controller.Move(dtoMove);
            Assert.IsType<NoContentResult>(resMove);

            var moved = await animalRepo.GetAsync(animal.Id);
            Assert.Equal(dst.Id, moved.EnclosureId);
        }
    }

    /// <summary>
    /// Проверяем работу StatisticsController:
    /// - получение общей статистики
    /// </summary>
    public class StatisticsControllerTests
    {
        [Fact]
        public async Task GetStats_Works()
        {
            var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
            var enclRepo = new InMemoryRepository<Enclosure>(e => e.Id);

            // добавляем одно животное
            var species = Species.Create("Giraffe", false);
            var food = new FoodType("Leaves");
            var animal = new Animal(
                species,
                "Ginny",
                DateOnly.FromDateTime(DateTime.Today),
                'F',
                food
            );
            await animalRepo.AddAsync(animal);

            // и один вольер
            var encl = new Enclosure("Herbivore", 200, 5);
            await enclRepo.AddAsync(encl);

            var service = new ZooStatisticsService(animalRepo, enclRepo);
            var controller = new StatisticsController(service);

            var getRes = await controller.Get();
            var okRes = Assert.IsType<OkObjectResult>(getRes);
            var dto = Assert.IsType<ZooStatsDto>(okRes.Value);

            Assert.Equal(1, dto.TotalAnimals);
            Assert.Equal(0, dto.PredatorEnclosuresFree);
            Assert.Equal(5, dto.HerbivoreEnclosuresFree);
        }
    }
}
