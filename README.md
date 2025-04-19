# Отчёт по проекту «Zoo API»

## Введение
Этот проект — **ASP.NET Core Web API** для автоматизации процессов зоопарка:
- CRUD‑операции над животными и вольерами  
- Планирование и отметка кормлений  
- Перемещение животных между вольерами  
- Сбор статистики  

Архитектура выстроена по **Clean Architecture** и использует **Domain‑Driven Design**.

---

## 1. Реализованный функционал

| Use Case                                                    | Файлы / модули                                                                                                                                       |
|-------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|
| **a. Добавить / удалить животное**                          | • `Controllers/AnimalsController.cs`<br>• `Application/IRepository.cs`<br>• `InMemoryRepository.cs`<br>• `Domain/Animal.cs`                       |
| **b. Добавить / удалить вольер**                            | • `Controllers/EnclosuresController.cs`<br>• `Application/IRepository.cs`<br>• `InMemoryRepository.cs`<br>• `Domain/Enclosure.cs`                  |
| **c. Переместить животное между вольерами**                 | • `Controllers/TransfersController.cs`<br>• `Application/AnimalTransferServ.cs`<br>• `Application/IAnimalTransferServ.cs`<br>• `Domain/Animal.cs`  |
| **d. Просмотреть расписание кормления**                     | • `Domain/FeedingSchedule.cs` (свойства `AnimalId`, `Time`, `Food`)<br>• хранение в `InMemoryRepository.cs`                                         |
| **e. Добавить новое кормление в расписание**                | • `Controllers/FeedingController.cs`<br>• `Application/FeedingOrganizationServ.cs`<br>• `Application/IFeedingOrganizationServ.cs`<br>• `Domain/FeedingSchedule.cs` |
| **f. Просмотреть статистику зоопарка**                      | • `Controllers/StatisticsController.cs`<br>• `Application/ZooStatisticsServ.cs`<br>• `Application/IZooStatisticsServ.cs`                             |

---

## 2. Применённые DDD‑концепции

- **Value Objects**  
  • `Domain/Species.cs`<br>• `Domain/FoodType.cs`  

- **Entities**  
  • `Domain/Animal.cs`<br>• `Domain/Enclosure.cs`<br>• `Domain/FeedingSchedule.cs`  

- **Domain Events**  
  • `Domain/AnimalMovedEvent.cs`<br>• `Domain/FeedingTimeEvent.cs`  

- **Инкапсуляция бизнес‑правил**  
  • Внутри методов `Animal.MoveTo()`, `Enclosure.Add()/Remove()`, `FeedingSchedule.MarkCompleted()`  

- **Repository Pattern**  
  • Интерфейс `Application/IRepository.cs`<br>• Реализация `InMemoryRepository.cs`  

---

## 3. Принципы Clean Architecture

1. **Слои зависят только внутрь**  
   - Presentation → Application → Domain  
   - Domain не зависит ни от чего

2. **Зависимости через абстракции (интерфейсы)**  
   - Все внешние на Domain и Application слои ссылаются только на интерфейсы (`IRepository`, `IAnimalTransferServ`, `IFeedingOrganizationServ`, `IZooStatisticsServ`).

3. **Изоляция бизнес‑логики**  
   - Вся логика содержится в слоях **Domain** и **Application**.  
   - Presentation и Infrastructure отвечают только за ввод/вывод и интеграцию.

4. **Внедрение зависимостей (DI)**  
   - `Program.cs` регистрирует `IRepository<T>` и сервисы (`AnimalTransferServ`, `FeedingOrganizationServ`, `ZooStatisticsServ`) в контейнере.

5. **Отсутствие циклических ссылок**  
   - Presentation зависит от Application  
   - Application зависит от Domain и абстракций  
   - Infrastructure зависит только от Application абстракций  

```text
+----------------------+      +----------------------+      +----------------------+
|      Controllers     | ---> |     Application      | ---> |        Domain        |
|  (Web API / .cs)     |      | (Services / DTOs)    |      | (Entities / VOs / Events) |
+----------------------+      +----------------------+      +----------------------+
         ^                             ^                            |
         |                             |                            v
         +-----------------------------+                  +----------------------+
           Program.cs & DI             Infrastructure     |  InMemoryRepository  |
                                                    (Implements IRepository<T>)  
                                                 +----------------------+
