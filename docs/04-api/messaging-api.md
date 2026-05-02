# Messaging API

Status: planned.

The current backend has messaging, conversation, reaction, read receipt,
threading, deletion, pinning, sticker, GIF, and media attachment entities with
EF Core mappings. HTTP endpoints and application services are not implemented
yet.

Planned endpoints:

- `GET /api/conversations/{conversationId}/messages`
- `POST /api/conversations/{conversationId}/messages`
- `PUT /api/messages/{id}`
- `DELETE /api/messages/{id}`
- `POST /api/messages/{id}/reactions`
- `DELETE /api/messages/{id}/reactions/{emoji}`
- `POST /api/messages/{id}/read`
- `POST /api/conversations/{conversationId}/messages/{id}/pin`
- `DELETE /api/conversations/{conversationId}/messages/{id}/pin`

Implementation tasks:

- Add message DTOs and validators.
- Add services for sequencing, send/edit/delete rules, reactions, reads, and pins.
- Wire authorization policies into endpoint handlers.
- Add pagination and ordering for message history.
- Add integration tests for conversation membership and message permissions.
