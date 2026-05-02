# Upcoming Backend Features - Quick Reference

This checklist tracks backend features that are planned or partially built.

Last updated: May 2, 2026

---

## Current Foundation

- [x] Domain entities for users, conversations, messages, channels, media, GIFs, stickers, moderation, and social features
- [x] EF Core `AppDbContext` mappings for the implemented domain model
- [x] GIF metadata mapping and recent GIF mapping
- [x] Role/permission configuration and seeding
- [x] Selected authorization policy classes
- [ ] API controllers/endpoints
- [ ] Application services and DTOs
- [ ] JWT authentication runtime
- [ ] SignalR/WebSocket runtime
- [ ] Tests

---

## 1. API Layer Completion

Priority: high

Estimated effort: 2-4 weeks

Backend tasks:

- [ ] Add controllers or minimal API endpoint modules
- [ ] Add request/response DTOs
- [ ] Add validation
- [ ] Add global error responses
- [ ] Wire existing authorization policies into endpoints
- [ ] Add pagination patterns
- [ ] Add integration tests

Primary endpoint groups:

- [ ] Auth
- [ ] Users
- [ ] Conversations
- [ ] Messages
- [ ] Channels
- [ ] Media
- [ ] GIFs
- [ ] Stickers

Related docs:

- [Current Status](CURRENT_STATUS.md)
- [API docs](04-api/auth.md)

---

## 2. GIF System API

Priority: high

Estimated effort: 1-2 weeks

Current status: entities and DbContext mappings are done.

Remaining tasks:

- [ ] GIF search endpoint
- [ ] Trending GIF endpoint
- [ ] Recent GIF endpoint
- [ ] Service for usage-count updates
- [ ] Service for recent GIF deduplication and ordering
- [ ] Tests for `GifMetadata` and `UserRecentGif` flows

Related docs:

- [GIF implementation guide](IMPLEMENTATION_GUIDE_GIF_SYSTEM.md)
- [Media API](04-api/media-api.md)

---

## 3. Authentication Runtime

Priority: high

Estimated effort: 1-2 weeks

Tasks:

- [ ] Password hashing service
- [ ] Register endpoint
- [ ] Login endpoint
- [ ] Refresh token endpoint
- [ ] Logout endpoint
- [ ] JWT bearer configuration
- [ ] ASP.NET Core authentication/authorization middleware
- [ ] Tests for login, lockout, token refresh, and logout

Related docs:

- [Authentication API](04-api/auth.md)
- [Authorization Model](02-policies/authorization-model.md)

---

## 4. Logging System

Priority: medium

Estimated effort: 1-2 weeks

Tasks:

- [ ] Structured application logging
- [ ] Request ID and user context enrichment
- [ ] Error logging middleware
- [ ] Performance timing for endpoints
- [ ] Log retention policy
- [ ] Sensitive data redaction

---

## 5. Report System

Priority: high

Estimated effort: 2-3 weeks

Tasks:

- [ ] Report entity and migration
- [ ] Report creation endpoints
- [ ] Report reason categories
- [ ] Admin/moderator review workflow
- [ ] Auto-moderation threshold rules
- [ ] Report history
- [ ] Appeal support

Reportable targets:

- [ ] Users
- [ ] Messages
- [ ] Conversations
- [ ] Channels
- [ ] Media

---

## 6. Admin Backend

Priority: high

Estimated effort: 3-4 weeks

Tasks:

- [ ] User search/filter endpoints
- [ ] User suspension and ban endpoints
- [ ] Role and permission management endpoints
- [ ] Report queue endpoints
- [ ] Content moderation endpoints
- [ ] Audit log endpoints
- [ ] System configuration endpoints

---

## 7. Real-Time Messaging

Priority: medium

Estimated effort: 2-4 weeks

Tasks:

- [ ] Add SignalR package and hub
- [ ] Authenticate hub connections
- [ ] Join users to conversation/channel groups
- [ ] Emit message created/edited/deleted events
- [ ] Emit reaction/read/presence events
- [ ] Add reconnect and delivery behavior docs

Related docs:

- [WebSocket architecture](03-realtime/websocket-architecture.md)
- [Events](03-realtime/events.md)

---

## 8. Appearance Customization

Priority: medium

Estimated effort: 1-2 weeks

Backend tasks:

- [ ] User appearance entity
- [ ] Theme preset entity or config
- [ ] Appearance endpoints
- [ ] Validation for colors, fonts, and media references

---

## 9. Bot System

Priority: medium

Estimated effort: 3-4 weeks

Tasks:

- [ ] Bot registration
- [ ] Bot API keys
- [ ] Bot permissions
- [ ] Webhook event delivery
- [ ] Rate limiting
- [ ] Bot usage analytics

---

## 10. Call And Streaming

Priority: medium

Estimated effort: 4-6 weeks

Tasks:

- [ ] Call entities
- [ ] Call history
- [ ] Signaling endpoints or hub methods
- [ ] STUN/TURN configuration
- [ ] WebRTC signaling flow
- [ ] Recording metadata support, if recording is allowed

---

## 11. End-To-End Encryption

Priority: high

Estimated effort: 3-4+ weeks

Tasks:

- [ ] Choose vetted cryptography library/protocol
- [ ] Key model
- [ ] Key exchange flow
- [ ] Encrypted message storage
- [ ] Key rotation
- [ ] Backward compatibility plan
- [ ] Security review

---

## Recommended Order

1. API layer completion
2. Authentication runtime
3. GIF API
4. Logging system
5. Report system
6. Real-time messaging
7. Admin backend
8. Appearance customization
9. Bot system
10. Call and streaming
11. End-to-end encryption
