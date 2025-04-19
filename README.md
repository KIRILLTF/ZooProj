# Отчёт по проекту «Zoo API»

## 1. Реализованный функционал

Ниже перечислены пункты из исходного технического задания и ссылки на файлы (классы/модули), где они реализованы.

| Use Case                                                                 | Реализация (классы / модули)                                                                                                                                         |
|---------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **a. Добавить / удалить животное**                                        | • `Presentation/Controllers/AnimalsController.cs`<br>• `Application/Abstractions/IRepository<Animal>`<br>• `Infrastructure/Persistence/InMemoryRepository<Animal>`<br>• `Domain/Entities/Animal.cs` |
| **b. Добавить / удалить вольер**                                          | • `Presentation/Controllers/EnclosuresController.cs`<br>• `Application/Abstractions/IRepository<Enclosure>`<br>• `Infrastructure/Persistence/InMemoryRepository<Enclosure>`<br>• `Domain/Entities/Enclosure.cs` |
| **c. Переместить животное между вольерами**                               | • `Presentation/Controllers/TransfersController.cs`<br>• `Application/Animals/AnimalTransferService.cs`<br>• `Domain/Entities/Animal.cs` (метод `MoveTo`)<br>• `Domain/Entities/Enclosure.cs` (`Add`/`Remove`) |
| **d. Просмотреть расписание кормления**                                   | • (хранение в `Infrastructure/Persistence/InMemoryRepository<FeedingSchedule>`)<br>• `Domain/Entities/FeedingSchedule.cs` (свойства `AnimalId`, `Time`, `Food`) |
| **e. Добавить новое кормление в расписание**                              | • `Presentation/Controllers/FeedingController.cs` (метод `POST /api/feeding`)<br>• `Application/Feeding/FeedingOrganizationService.cs`<br>• `Domain/Entities/FeedingSchedule.cs` |
| **f. Просмотреть статистику зоопарка** (кол‑во животных, свободные вольеры) | • `Presentation/Controllers/StatisticsController.cs`<br>• `Application/Statistics/ZooStatisticsService.cs`<br>• `Application/Statistics/IZooStatisticsService.cs`                            |

> **Примечание:** «просмотр расписания кормления» (d) можно расширить, добавив `GET /api/feeding`, аналогично контроллерам для животных и вольеров.

---

## 2. Применённые концепции Domain‑Driven Design и принципы Clean Architecture

| Концепция / принцип                                 | Где реализовано                                                                                |
|-----------------------------------------------------|------------------------------------------------------------------------------------------------|
| **Value Object**                                    | • `Domain/ValueObjects/Species.cs`<br>• `Domain/ValueObjects/FoodType.cs`                      |
| **Entity**                                          | • `Domain/Entities/Animal.cs`<br>• `Domain/Entities/Enclosure.cs`<br>• `Domain/Entities/FeedingSchedule.cs` |
| **Domain Event**                                    | • `Domain/Events/AnimalMovedEvent.cs`<br>• `Domain/Events/FeedingTimeEvent.cs`                 |
| **Инкапсуляция бизнес‑правил**                      | • Внутри `Animal.MoveTo()`, `Enclosure.Add()`, `FeedingSchedule.MarkCompleted()`               |
| **Repository (абстракция)**                         | • `Application/Abstractions/IRepository<T>`<br>• конкретная реализация `InMemoryRepository<T>` |
| **Сервисы приложения (Use Cases / Interactors)**    | • `Application/Animals/AnimalTransferService`<br>• `Application/Feeding/FeedingOrganizationService`<br>• `Application/Statistics/ZooStatisticsService` |
| **Чистая архитектура (слоёвая изоляция)**           | 1. **Domain** (ядро без внешних зависимостей)<br>2. **Application** (бизнес‑логика, зависит только от Domain и абстракций)<br>3. **Infrastructure** (реализация репозиториев и внешних интеграций)<br>4. **Presentation** (ASP.NET Core Web API, зависит только от Application) |
| **Внедрение зависимостей через интерфейсы (DI)**    | • `Program.cs`: регистрация `IRepository<T>` и сервисов (`AddScoped<I…Service, …Service>`)      |
| **Изоляция бизнес‑логики**                          | • Все правила — в `Domain` и `Application`; `Presentation` и `Infrastructure` не содержат бизнес‑правил |
| **Гранулярность единиц тестирования**               | • Unit‑тесты xUnit для сервисов и контроллеров (папка `ZooTests/`)                              |

