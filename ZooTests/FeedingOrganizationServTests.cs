using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Zoo.Application.Feeding;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Persistence;

namespace ZooTests.Services;

public class FeedingOrganizationServiceTests
{
    [Fact]
    public async Task AddScheduleAsync_CreatesSchedule()
    {
        var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
        var scheduleRepo = new InMemoryRepository<FeedingSchedule>(f => f.Id);

        var species = Species.Create("Bear", true);
        var food = new FoodType("Berries");
        var animal = new Animal(species, "Baloo", DateOnly.FromDateTime(DateTime.Today), 'M', food);
        await animalRepo.AddAsync(animal);

        var svc = new FeedingOrganizationService(animalRepo, scheduleRepo);
        var sched = await svc.AddScheduleAsync(animal.Id, new TimeOnly(9, 30));

        Assert.Equal(animal.Id, sched.AnimalId);
        Assert.Equal(food, sched.Food);
        Assert.False(sched.Completed);
    }

    [Fact]
    public async Task MarkCompletedAsync_SetsCompletedAndEmitsEvent()
    {
        var animalRepo = new InMemoryRepository<Animal>(a => a.Id);
        var scheduleRepo = new InMemoryRepository<FeedingSchedule>(f => f.Id);

        var species = Species.Create("Seal", false);
        var food = new FoodType("Fish");
        var animal = new Animal(species, "Sammy", DateOnly.FromDateTime(DateTime.Today), 'F', food);
        await animalRepo.AddAsync(animal);

        var svc = new FeedingOrganizationService(animalRepo, scheduleRepo);
        var sched = await svc.AddScheduleAsync(animal.Id, new TimeOnly(8, 0));

        await svc.MarkCompletedAsync(sched.Id);

        var updated = await scheduleRepo.GetAsync(sched.Id);
        Assert.True(updated.Completed);
        Assert.Single(updated.DomainEvents);
        Assert.IsType<Zoo.Domain.Events.FeedingTimeEvent>(updated.DomainEvents.First());
    }
}
