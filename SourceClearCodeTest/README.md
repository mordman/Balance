# Product Management System

Консольное приложение для управления продуктами с использованием чистой архитектуры, SQLite и поддержкой транзакций.

## Структура проекта

Проект разделен на несколько слоев в соответствии с принципами чистой архитектуры:

```
SourceClearCodeTest/
├── SourceClearCodeTest.Core/              # Слой предметной области
│   ├── Entities/                          # Доменные сущности
│   │   └── Product.cs                     # Сущность продукта
│   └── Interfaces/                        # Интерфейсы репозиториев и UnitOfWork
│       ├── IRepository.cs                 # Базовый интерфейс репозитория
│       └── IUnitOfWork.cs                 # Интерфейс UnitOfWork
│
├── SourceClearCodeTest.Infrastructure/    # Слой инфраструктуры
│   ├── Data/                              # Контекст базы данных (Справочники и т.п.)
│   │   └── ApplicationDbContext.cs        # DbContext для Entity Framework
│   └── Repositories/                      # Реализации репозиториев
│       ├── ProductRepository.cs           # Репозиторий для работы с продуктами
│       └── UnitOfWork.cs                  # Реализация паттерна Unit of Work
│
├── SourceClearCodeTest.Application/       # Слой бизнес-логики
│   ├── Interfaces/                        # Интерфейсы сервисов
│   │   └── IProductService.cs             # Интерфейс сервиса продуктов
│   └── Services/                          # Реализации сервисов
│       └── ProductService.cs              # Сервис для работы с продуктами
│
├── SourceClearCodeTest.ConsoleApp/        # Консольное приложение
│   └── Program.cs                         # Точка входа и UI
│
└── SourceClearCodeTest.Tests/             # Модульные тесты
    ├── ProductServiceTests.cs             # Тесты сервиса продуктов
    └── ProductRepositoryTests.cs          # Тесты репозитория продуктов
```

## Описание слоев

### Core (SourceClearCodeTest.Core)
- Содержит доменные сущности и бизнес-правила
- Определяет интерфейсы для работы с данными
- Не имеет зависимостей от других слоев
- Классы:
  - `Product`: основная сущность продукта
  - `IRepository<T>`: базовый интерфейс репозитория
  - `IProductRepository`: специализированный интерфейс для продуктов
  - `IUnitOfWork`: интерфейс для управления транзакциями

### Infrastructure (SourceClearCodeTest.Infrastructure)
- Реализует доступ к базе данных
- Содержит конкретные реализации репозиториев
- Использует Entity Framework Core и SQLite
- Классы:
  - `ApplicationDbContext`: контекст базы данных
  - `ProductRepository`: реализация репозитория продуктов
  - `UnitOfWork`: реализация паттерна Unit of Work для транзакций

### Application (SourceClearCodeTest.Application)
- Реализует бизнес-логику приложения
- Координирует работу между слоями
- Содержит сервисы приложения
- Классы:
  - `IProductService`: интерфейс сервиса продуктов
  - `ProductService`: реализация сервиса продуктов

### ConsoleApp (SourceClearCodeTest.ConsoleApp)
- Представляет пользовательский интерфейс
- Обрабатывает пользовательский ввод
- Внедряет зависимости
- Файлы:
  - `Program.cs`: точка входа и консольное меню

### Tests (SourceClearCodeTest.Tests)
- Содержит модульные тесты
- Использует MSTest и Moq
- Включает тесты бизнес-логики и интеграционные тесты
- Файлы:
  - `ProductServiceTests.cs`: тесты сервиса с моками
  - `ProductRepositoryTests.cs`: интеграционные тесты с in-memory SQLite

## Технологии

- .NET 8.0
- Entity Framework Core
- SQLite
- MSTest
- Moq

## Основные возможности

1. CRUD операции с продуктами:
   - Создание новых продуктов
   - Чтение информации о продуктах
   - Обновление существующих продуктов
   - Удаление продуктов

2. Дополнительные функции:
   - Поиск продуктов по диапазону цен
   - Обновление количества продуктов
   - Поддержка транзакций для всех операций

## Запуск приложения

```bash
cd SourceClearCodeTest.ConsoleApp
dotnet run
```

## Запуск тестов

```bash
cd SourceClearCodeTest
dotnet test
```

## Архитектурные особенности

1. **Чистая архитектура**
   - Слабая связанность между компонентами
   - Независимость от конкретных реализаций
   - Легкость в тестировании

2. **Паттерны**
   - Repository Pattern для доступа к данным
   - Unit of Work для управления транзакциями
   - Dependency Injection для внедрения зависимостей

3. **Принципы SOLID**
   - Single Responsibility Principle: каждый класс имеет одну ответственность
   - Open/Closed Principle: расширяемость без изменения существующего кода
   - Liskov Substitution Principle: использование интерфейсов
   - Interface Segregation Principle: специализированные интерфейсы
   - Dependency Inversion Principle: зависимость от абстракций
