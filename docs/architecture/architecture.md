# Архитектура

Сервисы:
- AuthService — отвечает за аутентификацию и выдачу токенов.
- UserService — управляет пользователями, их профилями.
- ProjectService — проекты, задачи и т.д.

Взаимодействие:
- Синхронно через HTTP (службы могут вызывать REST друг друга).
- Асинхронно через Event Bus (RabbitMQ).

Базы данных:
- AuthService -> PostgreSQL (authdb)
- UserService -> PostgreSQL (userdb)
- ProjectService -> PostgreSQL (projectdb)

GatewayService:
- Выполняет роль API Gateway, маршрутизируя запросы к нужным сервисам.
- Аггрегирует Swagger схемы с сервисов для единого UI.
