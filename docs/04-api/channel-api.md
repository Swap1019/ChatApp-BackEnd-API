# Channel And Conversation API

Status: planned.

The current backend contains channel and conversation entities, membership
entities, admin permission entities, URL history, bans, and authorization policy
classes. HTTP endpoints are not implemented yet.

Planned conversation endpoints:

- `POST /api/conversations`
- `GET /api/conversations`
- `GET /api/conversations/{id}`
- `PUT /api/conversations/{id}`
- `DELETE /api/conversations/{id}`
- `POST /api/conversations/{id}/members`
- `DELETE /api/conversations/{id}/members/{userId}`
- `POST /api/conversations/{id}/bans`
- `DELETE /api/conversations/{id}/bans/{userId}`

Planned channel endpoints:

- `POST /api/channels`
- `GET /api/channels`
- `GET /api/channels/{id}`
- `PUT /api/channels/{id}`
- `DELETE /api/channels/{id}`
- `POST /api/channels/{id}/subscribers`
- `DELETE /api/channels/{id}/subscribers/{userId}`

Implementation tasks:

- Split this document into separate channel and conversation API references once controllers exist.
- Add DTOs, services, validators, and authorization checks.
- Add tests for admin-only actions, bans, and private access rules.
