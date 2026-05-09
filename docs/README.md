# ChatApp Backend Documentation

## Quick Navigation

### Getting Started

- [System Overview](00-overview/system-overview.md) - Current backend architecture and implementation status
- [Architecture Design](00-overview/architecture.md) - Design patterns, layers, and architectural decisions
- [Current Status](CURRENT_STATUS.md) - What is built and what is pending

### Domain And Entities

- [Entity Design Guide](01-domain/entities-guide.md) - Why entities are designed this way

### Policies And Rules

- [Messaging Policies](02-policies/messaging-policies.md) - Planned and modeled message rules
- [Authorization Model](02-policies/authorization-model.md) - RBAC, roles, permissions, and access control
- [Privacy Policies](02-policies/privacy-policies.md) - Planned privacy decision layer, user settings, and exceptions
- [Channel Policies](02-policies/channel-policies.md) - Planned channel action policies and governance

### Real-Time Communication

- [WebSocket Architecture](03-realtime/websocket-architecture.md) - Planned SignalR architecture
- [Events Documentation](03-realtime/events.md) - Planned event types and payloads
- [Message Flow](03-realtime/message-flow.md) - Planned data flow

### API Reference

These pages currently document planned API contracts. The backend does not yet
contain controllers or minimal API endpoints.

- [Authentication API](04-api/auth.md) - Planned login, register, and token endpoints
- [Messaging API](04-api/messaging-api.md) - Planned message endpoints
- [Channel and Conversation API](04-api/channel-api.md) - Planned channel and conversation endpoints
- [Media API](04-api/media-api.md) - Planned file, GIF, and media endpoints

### Database

- [Schema Overview](05-database/schema-overview.md) - Database tables and relationships
- [DBML Schema Source](05-database/chatapp-schema.dbml) - Current dbdiagram source generated from entities and `AppDbContext`
- [Migrations Strategy](05-database/migrations-strategy.md) - EF Core migration approach
- [Indexing Strategy](05-database/indexing-strategy.md) - Performance indexes

### Performance And Scaling

- [Caching Strategy](06-performance/caching-strategy.md) - Planned caching approach
- [Bottlenecks And Solutions](06-performance/bottlenecks.md) - Known risks and mitigations
- [Scaling Strategy](06-performance/scaling.md) - Future scaling strategy

### Architecture Decisions

- [Avatar Design](07-decisions-log/avatar-design.md)
- [Channel vs Conversation](07-decisions-log/channel-vs-conversation.md)
- [Message Unification](07-decisions-log/message-unification.md)

### Feature Roadmap

- [Feature Roadmap](08-feature-roadmap.md)
- [Upcoming Features Checklist](UPCOMING_FEATURES_CHECKLIST.md)

---

## Backend Status

### Completed

- Domain entities for user management, auth/session data, RBAC, privacy, messaging, conversations, channels, media, GIFs, stickers, moderation, and social features
- EF Core `AppDbContext` relationship mapping
- Initial migrations
- Config-based role and permission seeding
- Authorization policy classes for selected message and conversation actions
- Policy specifications for channel actions and the user privacy decision layer

### Pending

- API controllers or minimal endpoints
- Application services and DTO mapping
- JWT authentication runtime
- SignalR/WebSocket runtime
- Structured logging pipeline
- Tests
- Admin panel API/UI

---

## Documentation Standards

- Keep status language precise: say "implemented" only for code that exists.
- Mark API docs as planned until controllers/endpoints are added.
- Update `CURRENT_STATUS.md` when domain, API, or infrastructure work changes.
- Link related docs with Markdown links.
- Prefer plain ASCII diagrams and text.

---

Last updated: May 9, 2026
