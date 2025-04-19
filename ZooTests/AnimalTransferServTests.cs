using System;
using System.Threading.Tasks;
using Xunit;
using Zoo.Application.Animals;
using Zoo.Application.Abstractions;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Persistence;

namespace ZooTests.Services;

public class AnimalTransferServiceTests
{
    [Fact]
    public async Task MoveAsync_Success()
    {
        var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
        var enclosureRepo = new InMemoryRepository<Enclosure>(e => e.Id);

        var species = Species.Create("Lion", true);
        var food = new FoodType("Meat");
        var animal = new Animal(species, "Simba", DateOnly.FromDateTime(DateTime.Today), 'M', food);

        var src = new Enclosure("Predator", 100, 2);
        src.Add(animal.Id);
        animal.MoveTo(src.Id);

        await animalRepo.AddAsync(animal);
        await enclosureRepo.AddAsync(src);

        var dst = new Enclosure("Predator", 50, 5);
        await enclosureRepo.AddAsync(dst);

        var svc = new AnimalTransferService(animalRepo, enclosureRepo);
        await svc.MoveAsync(animal.Id, dst.Id);

        var moved = await animalRepo.GetAsync(animal.Id);
        Assert.Equal(dst.Id, moved.EnclosureId);

        var dstAfter = await enclosureRepo.GetAsync(dst.Id);
        Assert.Contains(animal.Id, dstAfter.AnimalIds);
    }

    [Fact]
    public async Task MoveAsync_Throws_WhenTargetFull()
    {
        var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
        var enclosureRepo = new InMemoryRepository<Enclosure>(e => e.Id);

        var species = Species.Create("Gazelle", false);
        var food = new FoodType("Grass");
        var animal = new Animal(species, "Gary", DateOnly.FromDateTime(DateTime.Today), 'M', food);

        var src = new Enclosure("Herbivore", 100, 1);
        src.Add(animal.Id);
        animal.MoveTo(src.Id);

        await animalRepo.AddAsync(animal);
        await enclosureRepo.AddAsync(src);

        var dst = new Enclosure("Herbivore", 30, 0);
        await enclosureRepo.AddAsync(dst);

        var svc = new AnimalTransferService(animalRepo, enclosureRepo);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.MoveAsync(animal.Id, dst.Id).AsTask());
    }

    [Fact]
    public async Task MoveAsync_Throws_WhenTypeMismatch()
    {
        var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
        var enclosureRepo = new InMemoryRepository<Enclosure>(e => e.Id);

        var species = Species.Create("Deer", false);
        var food = new FoodType("Leaves");
        var animal = new Animal(species, "Bambi", DateOnly.FromDateTime(DateTime.Today), 'F', food);

        var src = new Enclosure("Herbivore", 100, 2);
        src.Add(animal.Id);
        animal.MoveTo(src.Id);

        await animalRepo.AddAsync(animal);
        await enclosureRepo.AddAsync(src);

        var predEnc = new Enclosure("Predator", 80, 2);
        await enclosureRepo.AddAsync(predEnc);

        var svc = new AnimalTransferService(animalRepo, enclosureRepo);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => svc.MoveAsync(animal.Id, predEnc.Id).AsTask());
    }
}
