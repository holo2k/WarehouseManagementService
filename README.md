# WarehouseManagementService

**WarehouseManagementService** — это REST API-сервис для учета товаров на складе.

Сервис реализован на **.NET 10**, использует **PostgreSQL** и **Entity Framework**.

## Запуск

### Запуск из Visual Studio

В Visual Studio можно выбрать проект **Docker Compose** в качестве стартового проекта и запустить его через стандартный Debug/Run. (Должен быть запущен Docker Desktop)

Так будут запущены оба контейнера:

- `api` — REST API
- `db` — PostgreSQL

После запуска Swagger будет доступен по адресу:

[http://localhost:8080/swagger](http://localhost:8080/swagger)

### Запуск из консоли

1. Перейдите в папку проекта:

```bash
cd WarehouseManagementService
```

2. Запустите Docker Compose:

```bash
docker compose up -d --build
```

3. После запуска сервис будет доступен по адресу:

[http://localhost:8080/swagger](http://localhost:8080/swagger)

Docker Compose не требует `.env`: имена сервисов, порты и строка подключения уже заданы в `docker-compose.yml`.


## Локальный запуск без Docker

Нужен PostgreSQL с базой `warehouse_management` и пользователем `postgres/postgres`, либо своя строка подключения.


## Подключение к базе данных

При запуске через Docker Compose:

- host с хоста: `localhost`
- host внутри Docker-сети: `db`
- port: `5432`
- database: `warehouse_management`
- user: `postgres`
- password: `postgres`

## Информация об API

- Получение списка категорий
- Создание категории
- Получение списка товаров
- Фильтрация товаров по статусу и категории
- Пагинация списка товаров
- Получение товара по ID
- Создание товара
- Изменение статуса товара
- Проверка уникальности SKU
- Автоматическое создание БД через EF Core migrations
- Добавление тестовых данных при первом запуске
- Оптимистичная конкурентность при изменении статуса товара
- Единый JSON-формат успешных ответов и ошибок
- Swagger UI для ручного тестирования API

## Технологии, паттерны, решения

- ASP.NET Core 10
- Entity Framework Core
- PostgreSQL
- MediatR
- CQRS на уровне приложения
- Result pattern
- FluentValidation
- Optimistic Concurrency — поле `status` настроено как concurrency token, чтобы параллельные изменения статуса не перетирали друг друга
- Exception Handling Middleware — исключения автоматически преобразуются в JSON-ответы с корректным HTTP-статусом.
- AutoMapper
- Swagger
- Docker, Docker Compose
- xUnit

## Команды разработки

Сборка решения:

```cmd
dotnet build WarehouseManagementService.slnx
```

Запуск тестов:

```cmd
dotnet test WarehouseManagementService.slnx
```

## Использование Result

В проекте используется единый формат ответа `ApiResponse<T>`.

Успешный ответ:

```json
{
  "success": true,
  "data": {},
  "error": null
}
```

Ответ с ошибкой:

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "validation_error",
    "message": "Validation failed.",
    "details": {
      "Name": ["'Name' must not be empty."]
    }
  }
}
```

Основные коды ошибок:

| Код ошибки             | Описание                                                      |
|------------------------|---------------------------------------------------------------|
| `validation_error`     | Ошибка валидации входных данных                               |
| `not_found`            | Сущность не найдена                                           |
| `conflict`             | Конфликт данных (например, повторяющийся SKU)                 |
| `domain_rule_violation`| Нарушение доменного правила                                   |
| `concurrency_conflict` | Конфликт конкурентного изменения                              |
| `database_conflict`    | Ошибка ограничения базы данных                                |

## Статусы товара

Доступные статусы:

- `Active`
- `Defective`
- `WriteOff`

Допустимые переходы:

- `Active -> Defective`
- `Defective -> WriteOff`

Запрещены:

- обратные переходы
- прямой переход `Active -> WriteOff`



## Примеры curl-запросов

### Получить категории

```bash
curl -X GET "http://localhost:8080/api/categories"
```


### Создать категорию

```bash
curl -X POST "http://localhost:8080/api/categories" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Планшеты"
  }'
```


### Получить товары

```bash
curl -X GET "http://localhost:8080/api/products?page=1&pageSize=20"
```

### Получить товары с фильтрами

```bash
curl -X GET "http://localhost:8080/api/products?status=Active&categoryId=1&page=1&pageSize=10"
```


### Получить товар по ID

```bash
curl -X GET "http://localhost:8080/api/products/1"
```


### Создать товар

```bash
curl -X POST "http://localhost:8080/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Apple iPad 10",
    "sku": "TAB-APL-IPAD10-64",
    "categoryId": 1,
    "status": "Active"
  }'
```


### Изменить статус товара

```bash
curl -X PATCH "http://localhost:8080/api/products/1/status" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Defective"
  }'
```


## Структура проекта

```text
WarehouseManagementService
├── docker-compose.yml
├── docker-compose.override.yml
├── docker-compose.dcproj
├── WarehouseManagementService.slnx
├── src
│   ├── WarehouseManagementService.Api
│   │   ├── Bootstrap
│   │   ├── Controllers
│   │   ├── Middleware
│   │   ├── Properties
│   │   ├── Dockerfile
│   │   └── Program.cs
│   ├── WarehouseManagementService.Application
│   │   ├── Categories
│   │   ├── Common
│   │   └── Products
│   ├── WarehouseManagementService.Domain
│   │   ├── Entities
│   │   └── Enums
│   └── WarehouseManagementService.Infrastructure
│       ├── Persistence
│       │   ├── Configurations
│       │   ├── Initializer
│       │   └── Migrations
│       └── DependencyInjection.cs
└── tests
    └── WarehouseManagementService.Tests
```

## Контакты

* TG: [holo21k](https://t.me/holo21k)
* Email: [nneketaa@yandex.ru](mailto:nneketaa@yandex.ru)
