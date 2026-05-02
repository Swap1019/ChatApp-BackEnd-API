# ChatApp Backend System Overview

## Project Status

Current phase: backend domain model and database infrastructure.

Date: May 2, 2026

Architecture: Clean Architecture with Domain-Driven Design style organization.

The current backend has a substantial entity model, EF Core mapping, migrations,
permission seeding, and authorization policy classes. Runtime API behavior is
still early: `Program.cs` currently registers OpenAPI and `AppDbContext`, but it
does not yet register controllers, authentication middleware, SignalR, or
application services.

---

## Technology Stack

- .NET 10.0 / ASP.NET Core
- Entity Framework Core 10
- Npgsql Entity Framework Core provider
- PostgreSQL
- ASP.NET Core OpenAPI

---

## Backend Project Structure

```text
backend/
  ChatApp.Domain/          Domain entities, enums, and value objects
  ChatApp.Application/     Authorization policies and future services/DTOs
  ChatApp.Infrastructure/  DbContext, seeding, and infrastructure services
  ChatApp.API/             ASP.NET Core entry point and EF migrations
  ChatApp.Common/          Shared code placeholder
```

Planned but not yet present:

- API controllers or minimal endpoint modules
- Application DTOs and service implementations
- Repository implementations, if the project chooses that pattern
- SignalR hubs
- Authentication/authorization middleware setup

---

## Implemented Backend Areas

### User And Identity Model

- User profile and verification fields
- User sessions and tokens
- User privacy and privacy exceptions
- User suspensions
- Roles, permissions, role-permission mappings, and user-role mappings

### Conversation And Messaging Model

- One-to-one and group conversation entities
- Conversation membership and admin permission entities
- Conversation bans and URL history
- Messages with sequence ordering
- Message threads, reactions, reads, deletions, pins, media attachments, stickers, and GIF references

### Channel Model

- Channel entities
- Channel subscriptions
- Channel admin permissions
- Channel bans
- Channel URL history
- Channel pinned messages

### Media Model

- Unified `Media` entity for files
- Owned `MediaMetadata` value object
- Message and post attachments
- Avatars
- GIF metadata
- Recent GIF tracking
- Media moderation and sensitive-content flags

### Social Model

- Contacts
- Blocked users
- Posts and likes
- Stories and story views

---

## Core Design Patterns

### Soft Deletes

Many entities are marked deleted instead of physically removed. Global query
filters exclude soft-deleted rows by default.

### Composite Keys

Relationship tables use composite keys where duplicate associations should be
impossible, such as `(UserId, RoleId)` and `(UserId, MediaId)`.

### Unified Media Storage

Media storage is centralized in `Media`, with contextual entities such as
`MessageAttachment`, `PostAttachment`, `Avatar`, and `GifMetadata`.

### Authorization Policies

The application layer includes policy classes for several conversation and
message operations. These policies still need to be wired into API endpoints.

---

## Current Runtime Surface

Implemented in `ChatApp.API/Program.cs`:

- OpenAPI registration
- PostgreSQL `AppDbContext` registration
- HTTPS redirection
- Development OpenAPI endpoint

Not implemented yet:

- Controllers or minimal API endpoints
- JWT authentication
- ASP.NET Core authorization middleware
- SignalR/WebSocket hubs
- Request validation middleware
- Error handling middleware
- Logging middleware

---

## Next Steps

See [Feature Roadmap](../08-feature-roadmap.md) and
[Current Status](../CURRENT_STATUS.md) for implementation priorities.
