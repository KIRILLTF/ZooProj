Зайцев Кирилл Николаевич, БПИ238

# Отчёт по проекту «Zoo API»

## Введение
Этот проект представляет собой **ASP.NET Core Web API** для автоматизации ключевых процессов Московского зоопарка:  
- CRUD‑операции для животных и вольеров  
- Организация и отметка кормлений  
- Перемещение животных между вольерами  
- Сбор статистики по зоопарку  

---

## 1. Реализованный функционал

| Use Case                                                     | Где реализовано (классы/модули)                                                                                                                                           |
|--------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **a. Добавить / удалить животное**                           | • `Presentation/Controllers/AnimalsController.cs`<br>• `Application/Abstractions/IRepository<Animal>.cs`<br>• `Infrastructure/Persistence/InMemoryRepository<Animal>.cs`<br>• `Domain/Entities/Animal.cs` |
| **b. Добавить / удалить вольер**                             | • `Presentation/Controllers/EnclosuresController.cs`<br>• `Application/Abstractions/IRepository<Enclosure>.cs`<br>• `Infrastructure/Persistence/InMemoryRepository<Enclosure>.cs`<br>• `Domain/Entities/Enclosure.cs` |
| **c. Переместить животное между вольерами**                  | • `Presentation/Controllers/TransfersController.cs`<br>• `Application/Animals/AnimalTransferService.cs`<br>• `Domain/Entities/Animal.cs` (метод `MoveTo`)<br>• `Domain/Entities/Enclosure.cs` (`Add`/`Remove`) |
| **d. Просмотреть расписание кормления**                      | • `Domain/Entities/FeedingSchedule.cs` (свойства `AnimalId`, `Time`, `Food`)<br>• хранилище в `Infrastructure/Persistence/InMemoryRepository<FeedingSchedule>.cs`             |
| **e. Добавить новое кормление в расписание**                 | • `Presentation/Controllers/FeedingController.cs` (POST `/api/feeding`)<br>• `Application/Feeding/FeedingOrganizationService.cs`<br>• `Domain/Entities/FeedingSchedule.cs`     |
| **f. Просмотреть статистику зоопарка**                       | • `Presentation/Controllers/StatisticsController.cs`<br>• `Application/Statistics/ZooStatisticsService.cs`<br>• `Application/Statistics/IZooStatisticsService.cs`            |

---

## 2. Применённые концепции Domain‑Driven Design

- **Value Objects**  
  • `Domain/ValueObjects/Species.cs`  
  • `Domain/ValueObjects/FoodType.cs`  

- **Entities**  
  • `Domain/Entities/Animal.cs`  
  • `Domain/Entities/Enclosure.cs`  
  • `Domain/Entities/FeedingSchedule.cs`  

- **Domain Events**  
  • `Domain/Events/AnimalMovedEvent.cs`  
  • `Domain/Events/FeedingTimeEvent.cs`  

- **Инкапсуляция бизнес‑правил**  
  • Внутри методов `Animal.MoveTo()`, `Enclosure.Add()/Remove()`, `FeedingSchedule.MarkCompleted()`  

- **Repository Pattern**  
  • Интерфейс `Application/Abstractions/IRepository<T>.cs`  
  • Реализация `Infrastructure/Persistence/InMemoryRepository<T>.cs`  

---

## 3. Принципы Clean Architecture

**Layer 1: Domain**  
- Не зависит ни от какого внешнего кода.  
- Содержит модели и логику предметной области.  
- Папка: `Domain/`  
  - `Primitives/ValueObject.cs`  
  - `ValueObjects/Species.cs`, `FoodType.cs`  
  - `Entities/Animal.cs`, `Enclosure.cs`, `FeedingSchedule.cs`  
  - `Events/IDomainEvent.cs`, `AnimalMovedEvent.cs`, `FeedingTimeEvent.cs`  

**Layer 2: Application**  
- Зависит только от Domain и абстракций.  
- Описывает бизнес‑use‑cases.  
- Папка: `Application/`  
  - `Abstractions/IRepository<T>.cs`  
  - `Animals/IAnimalTransferService.cs`, `AnimalTransferService.cs`  
  - `Feeding/IFeedingOrganizationService.cs`, `FeedingOrganizationService.cs`  
  - `Statistics/IZooStatisticsService.cs`, `ZooStatisticsService.cs`  

**Layer 3: Infrastructure**  
- Зависит от Application абстракций.  
- Реализует внешние интеграции и хранилище.  
- Папка: `Infrastructure/Persistence/`  
  - `InMemoryRepository.cs`  

**Layer 4: Presentation**  
- Зависит только от Application.  
- Обеспечивает Web API.  
- Папка: `Presentation/Controllers/`  
  - `AnimalsController.cs`  
  - `EnclosuresController.cs`  
  - `FeedingController.cs`  
  - `TransfersController.cs`  
  - `StatisticsController.cs`  
- Файл: `Program.cs` (DI, Swagger, маршрутизация)

---

### Диаграмма слоёв

         ┌──────────────────────────┐
         │      Presentation        │
         │   (Web API / Controllers)│
         └───────────┬──────────────┘
                     │
                     ▼
    ┌───────────────────────────────────┐
    │           Application            │
    │ (Use‑Cases / Services / DTOs)    │
    └───────────┬──────────────┬───────┘
                │              │
                ▼              │
┌─────────────────────────┐     │
│         Domain          │     │
│ (Entities / Value Obj.  │     │
│  / Domain Events / Rules)◄────┘
└─────────────────────────┘
                ▲
                │
    ┌───────────┴──────────────┐
    │      Infrastructure      │
    │ (Repositories / Adapters)│
    └──────────────────────────┘
