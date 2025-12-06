# T-API - Rational Assistant

API для приложения осознанных покупок.

## Сервисы

| Сервис | Порт | Описание |
|--------|------|----------|
| CoolingService | 5001 | Периоды охлаждения |
| UserService | 5002 | Настройки пользователя |
| PortfolioService | 5003 | Цели покупок |
| HashService | 5004 | Хэширование и GUID |

## Запуск

```powershell
.\start-services.ps1
```

Docker:
```bash
docker-compose up --build
```

## Swagger UI

- http://localhost:5001/swagger
- http://localhost:5002/swagger
- http://localhost:5003/swagger
- http://localhost:5004/swagger

## Безопасность

- SQL-инъекции: EF Core параметризованные запросы
- XSS: HTML-экранирование входных данных
- Хэширование: SHA256/MD5 с солью
