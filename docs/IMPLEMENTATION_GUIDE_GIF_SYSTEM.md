# GIF System Implementation Guide

## Current Status

The GIF domain and database mapping are implemented. API behavior is still
planned.

Completed:

- [x] `Media` supports GIF files through `MediaType.Gif`
- [x] `GifMetadata` entity exists for title, tags, emoji hints, usage count, and trending state
- [x] `UserRecentGif` entity exists for recent GIF tracking
- [x] `AppDbContext` includes `GifMetadatas` and `UserRecentGifs`
- [x] `GifMetadata` is configured as one-to-one with `Media`
- [x] `UserRecentGif` uses composite key `(UserId, MediaId)`
- [x] `Message` can reference GIF media through `GifMediaId`

Remaining:

- [ ] GIF DTOs
- [ ] GIF service
- [ ] GIF search/trending/recent queries
- [ ] GIF API endpoints
- [ ] Tests

---

## Implemented Database Shape

`GifMetadata`:

- Primary key: `Id`
- Unique index: `MediaId`
- Indexes: `IsTrending`, `UsageCount`, `CreatedAt`, `IsDeleted`
- Relationship: one GIF metadata row belongs to one `Media` row

`UserRecentGif`:

- Primary key: `(UserId, MediaId)`
- Indexes: `UserId`, `MediaId`, `LastUsedAt`
- Relationship: many recent GIF rows per user
- Relationship: many recent GIF rows per media item

`Message`:

- Optional `GifMediaId`
- Relationship to `Media` with `DeleteBehavior.SetNull`

---

## Planned API

Suggested endpoints:

- `GET /api/gifs/search`
- `GET /api/gifs/trending`
- `GET /api/gifs/recent`
- `POST /api/gifs/{mediaId}/recent`
- `POST /api/media/gifs`

Suggested service responsibilities:

- Search by title, tags, and emoji hint.
- Return trending GIFs ordered by usage count.
- Track recent GIF usage per user.
- Increment GIF usage counts when used in messages.
- Validate that referenced media is a GIF.

---

## Implementation Checklist

1. Add GIF DTOs in the application layer.
2. Add `IGifService` and implementation.
3. Add query methods for search, trending, and recent GIFs.
4. Add API endpoints.
5. Wire authentication/authorization once runtime auth exists.
6. Add tests for search, recent tracking, and message GIF references.
