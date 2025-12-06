# Рациональный Ассистент API

**Хакатон Т-Банк 2025** - Микросервисная архитектура для управления импульсивными покупками

## Запуск

```bash
dotnet build
# Запустите 3 терминала:
cd CoolingService && dotnet run     # https://localhost:7001/swagger
cd UserService && dotnet run         # https://localhost:7002/swagger  
cd PortfolioService && dotnet run    # https://localhost:7003/swagger
```

## Архитектура

- **CoolingService** (7001) - Диапазоны охлаждения
- **UserService** (7002) - Настройки пользователей
- **PortfolioService** (7003) - Портфолио покупок + вызовы других сервисов

## API

**CoolingService:** `GET/POST /api/v1/cooling/cooling-ranges`  
**UserService:** `GET/PUT /api/v1/user/preferences`  
**PortfolioService:** `GET/POST/PUT/DELETE /api/v1/portfolio`

Все запросы требуют header: `X-User-Id: <userId>`

## Технологии

.NET 8.0 | ASP.NET Core | Swagger | In-Memory Storage
