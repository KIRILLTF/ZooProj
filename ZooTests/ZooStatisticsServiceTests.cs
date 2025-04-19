using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Zoo.Application.Statistics;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Persistence;

namespace ZooTests.Services;

public class ZooStatisticsServiceTests
{
    [Fact]
    public async Task GetAsync_CalculatesCorrectStats()
    {
        var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
        var enclosureRepo = new InMemoryRepository<Enclosure>(e => e.Id);

        var predSpecies = Species.Create("Tiger", true);
        var predFood = new FoodType("Meat");
        var predator = new Animal(predSpecies, "Tiggy", DateOnly.FromDateTime(DateTime.Today), 'F', predFood);

        var predEnc = new Enclosure("Predator", 100, 2);
        predEnc.Add(predator.Id);
        predator.MoveTo(predEnc.Id);

        await animalRepo.AddAsync(predator);
        await enclosureRepo.AddAsync(predEnc);

        var herbSpecies = Species.Create("Giraffe", false);
        var herbFood = new FoodType("Leaves");
        var herbivore = new Animal(herbSpecies, "Gerry", DateOnly.FromDateTime(DateTime.Today), 'M', herbFood);

        var herbEnc = new Enclosure("Herbivore", 200, 3);
        await animalRepo.AddAsync(herbivore);
        await enclosureRepo.AddAsync(herbEnc);

        var svc = new ZooStatisticsService(animalRepo, enclosureRepo);
        var stats = await svc.GetAsync();

        Assert.Equal(2, stats.TotalAnimals);
        Assert.Equal(1, stats.PredatorEnclosuresFree);
        Assert.Equal(3, stats.HerbivoreEnclosuresFree);
    }
}
