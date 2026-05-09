# Current Backend Implementation Summary

Last updated: May 9, 2026

This document reflects the current .NET backend codebase. The project currently
contains the domain model, EF Core DbContext configuration, migrations, role and
permission seeding, and authorization policy classes. HTTP controllers,
application services, DTO mapping, JWT authentication middleware, and real-time
messaging are still pending.

---

## What's Built

### Domain/Data Systems

- [x] User identity entities with verification fields
- [x] Session, token, privacy, role, permission, and suspension entities
- [x] RBAC role-permission and user-role mappings
- [x] Conversation, member, admin, URL, and ban entities
- [x] Channel, subscriber, admin, URL, ban, and pinned-message entities
- [x] Messaging entities for messages, threads, reads, reactions, deletions, pins, stickers, and attachments
- [x] Unified media entities for files, avatars, GIF metadata, moderation, and sensitive-content flags
- [x] Social entities for contacts, blocks, posts, likes, stories, and story views

### Database

- [x] PostgreSQL configured through EF Core/Npgsql
- [x] Entity relationships configured in `AppDbContext`
- [x] Query filters for soft deletes and revoked bans
- [x] Unique and composite indexes
- [x] Initial migration infrastructure
- [x] GIF mappings, including `GifMetadata` one-to-one with `Media`
- [x] `UserRecentGif` composite key: `(UserId, MediaId)`

### Architecture

- [x] Clean Architecture project layout
- [x] Domain-Driven Design style entity organization
- [x] Dependency injection for `AppDbContext`
- [x] Authorization policy classes for several conversation/message actions
- [x] Config-based permission seeding
- [x] Policy documentation for planned channel actions and privacy-layer checks

---

## Not Implemented Yet

- [ ] HTTP API controllers or minimal API endpoints
- [ ] Application services and DTO mapping
- [ ] JWT authentication and authorization middleware
- [ ] SignalR/WebSocket runtime
- [ ] Request validation and global error handling middleware
- [ ] Structured logging pipeline
- [ ] Unit/integration test projects
- [ ] Admin panel API/UI
- [ ] Channel action policy classes
- [ ] Runtime privacy decision service

---

## Next Priority Tasks

### Phase 1: API Foundation

1. **Create the API layer**
   - Add controllers or minimal APIs.
   - Add request/response DTOs.
   - Add validation and consistent error responses.
   - Wire existing authorization policies into endpoints.
   - Use the documented channel policies and privacy layer when adding channel
     and user-data endpoints.

2. **Implement core services**
   - `MessageService`
   - `ConversationService`
   - `UserService`
   - `MediaService`
   - `GifService`

3. **Authentication**
   - Configure JWT bearer authentication.
   - Implement register/login/refresh/logout flows.
   - Connect token/session entities to runtime auth behavior.

4. **GIF System Integration**
   - DbContext/entity work is done.
   - Remaining work: GIF API endpoints, search service, trending/recent flows.

### Phase 2: Operational Features

5. **Logging System**
   - Structured logging.
   - Request context and error logging.
   - Log persistence/retention strategy.

6. **Report System**
   - Report entities, endpoints, review queue, and moderation workflow.

7. **Real-Time Messaging**
   - SignalR hub.
   - Message delivery events.
   - Presence updates.

### Phase 3: Larger Features

8. Bot system
9. Admin panel
10. Call and streaming
11. End-to-end encryption

---

## Current Database Schema Highlights

### User Management

- `User`
- `UserSession`
- `UserToken`
- `UserSuspension`
- `UserPrivacy`
- `UserPrivacyException`
- `UserRole`

### Messaging

- `Message`
- `MessageAttachment`
- `MessageReaction`
- `MessageRead`
- `MessageDeletion`
- `MessageThread`
- `PinnedMessage`

### Conversations

- `Conversation`
- `ConversationUser`
- `ConversationUserAdmin`
- `ConversationUserBan`
- `ConversationUrl`

### Media

- `Media`
- `GifMetadata`
- `UserRecentGif`
- `Avatar`
- `MessageAttachment`
- `PostAttachment`
- `MediaModeration`
- `SensitiveContentFlag`

### Stickers

- `StickerPack`
- `Sticker`
- `UserRecentSticker`
- `UserSticker`

### Channels

- `Channel`
- `ChannelUser`
- `ChannelUserAdmin`
- `ChannelUserBan`
- `ChannelUrl`
- `ChannelPinnedMessage`

### Social

- `Contact`
- `BlockedUser`
- `Post`
- `PostLike`
- `Story`
- `StorySeen`

### Authorization

- `Role`
- `Permission`
- `RolePermission`

---

## Key Design Patterns In Use

1. **Soft deletes**: `IsDeleted` and `DeletedAt` fields preserve data.
2. **Query filters**: Global filters hide deleted or revoked rows by default.
3. **Composite keys**: Join tables prevent duplicate relationships.
4. **One-to-one via FK**: `UserPrivacy` and `GifMetadata` are keyed by related entities.
5. **Polymorphic media storage**: `Media` supports different file contexts.
6. **Audit fields**: `CreatedAt`, `UpdatedAt`, and `*ById` fields track changes.
7. **Value objects**: Media metadata is configured as an owned object.

---

## Development Metrics

- Domain entities: 47
- Enumerations: 15+
- Database tables: about 40+
- Documentation pages: 25+

---

## Contributing Checklist

When adding a backend feature:

1. Add or update domain entities.
2. Add relationships and indexes in `AppDbContext`.
3. Add migrations.
4. Add services and DTOs.
5. Add API endpoints.
6. Add authorization checks.
7. Add tests.
8. Update the relevant docs.
