using Microsoft.OpenApi.Models;
using Zoo.Application.Abstractions;
using Zoo.Application.Animals;
using Zoo.Application.Feeding;
using Zoo.Application.Statistics;
using Zoo.Domain.Entities;
using Zoo.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:53377");

// Регистрация in-memory репозиториев
builder.Services.AddSingleton<IRepository<Animal>>(
    _ => new InMemoryRepository<Animal>(a => a.Id));
builder.Services.AddSingleton<IRepository<Enclosure>>(
    _ => new InMemoryRepository<Enclosure>(e => e.Id));
builder.Services.AddSingleton<IRepository<FeedingSchedule>>(
    _ => new InMemoryRepository<FeedingSchedule>(f => f.Id));

// Регистрация сервисов бизнес‑логики
builder.Services.AddScoped<IAnimalTransferService, AnimalTransferService>();
builder.Services.AddScoped<IFeedingOrganizationService, FeedingOrganizationService>();
builder.Services.AddScoped<IZooStatisticsService, ZooStatisticsService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Zoo API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();