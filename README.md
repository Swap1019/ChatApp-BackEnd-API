# ChatApp Backend API

ChatApp Backend is a .NET rewrite of the original Django MVP. The current
repository contains the backend domain model, EF Core database mapping,
migrations, permission seeding, and authorization policy groundwork for a chat
and social communication platform.

The HTTP API layer is not complete yet. `ChatApp.API` currently registers
OpenAPI and the PostgreSQL `AppDbContext`; controllers/endpoints,
authentication middleware, application services, SignalR, and admin APIs are
planned work.

Previous Django backend MVP:
https://github.com/Swap1019/ChatApp-BackEnd

Database schema:
https://dbdiagram.io/d/ChatAppDbDesignDotnet-69d77ff70f7c9ef2c0b79147

---

## Implemented Backend Foundation

### Domain Model

- User identity, sessions, tokens, privacy, roles, permissions, and suspensions
- Conversations, members, admins, bans, URLs, pinned messages, and messages
- Message threading, reactions, read receipts, deletions, stickers, GIFs, and attachments
- Channels, subscribers, admins, bans, URL history, and pinned messages
- Unified media storage, avatars, moderation flags, sensitive-content flags, and GIF metadata
- Contacts, blocked users, posts, likes, stories, and story views

### Infrastructure

- PostgreSQL through Entity Framework Core/Npgsql
- `AppDbContext` entity relationships, indexes, owned metadata, and query filters
- Initial EF migration in `ChatApp.API`
- Role/permission configuration and seeders

### Application Layer

- Authorization policy classes for selected conversation and message operations
- Policy result abstractions

---

## Pending Backend Work

- API controllers or minimal API endpoints
- Request/response DTOs
- Application services
- JWT authentication and authorization middleware
- Password hashing and token issuance flows
- SignalR/WebSocket runtime for real-time messaging
- Structured logging middleware/persistence
- Unit and integration tests
- Admin/reporting APIs

---

## Project Structure

```text
backend/
  ChatApp.Domain/          Entities, enums, and value objects
  ChatApp.Application/     Authorization policies and future services/DTOs
  ChatApp.Infrastructure/  DbContext, seeding, and infrastructure services
  ChatApp.API/             ASP.NET Core entry point and migrations
  ChatApp.Common/          Shared code placeholder
```

---

## Technology Stack

- .NET 10.0
- ASP.NET Core
- Entity Framework Core 10
- Npgsql Entity Framework Core provider
- PostgreSQL
- ASP.NET Core OpenAPI

---

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL
- Git

### Restore

```bash
cd BackEnd/backend
dotnet restore
```

### Configure Database

Update `ChatApp.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ChatAppDb;Username=postgres;Password=your-password"
  }
}
```

### Apply Migrations

```bash
dotnet ef database update --project ChatApp.API
```

### Run The API Host

```bash
dotnet run --project ChatApp.API
```

Development URLs from `launchSettings.json`:

- `http://localhost:5210`
- `https://localhost:7074`

In development, the OpenAPI document is mapped by `MapOpenApi()`.

---

## Documentation

- [Backend docs index](docs/README.md)
- [Current backend status](docs/CURRENT_STATUS.md)
- [System overview](docs/00-overview/system-overview.md)
- [Feature roadmap](docs/08-feature-roadmap.md)
- [Upcoming features checklist](docs/UPCOMING_FEATURES_CHECKLIST.md)
- [Permission customization guide](backend/ChatApp.Infrastructure/Seeding/PERMISSION_CUSTOMIZATION_GUIDE.md)

---

## Notes For Contributors

When adding a backend feature:

1. Update or add domain entities.
2. Configure relationships and indexes in `AppDbContext`.
3. Add migrations.
4. Add services, DTOs, and validation.
5. Add API endpoints.
6. Add authorization checks.
7. Add tests.
8. Update docs to match the code.

---

## License

This project is licensed under the MIT License. See `LICENSE` for details.
