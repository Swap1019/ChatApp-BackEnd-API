# Media API

Status: planned.

The current backend has media, avatar, moderation, sensitive-content, attachment,
GIF metadata, and recent GIF entities with EF Core mappings. Upload, processing,
search, and moderation endpoints are not implemented yet.

Planned endpoints:

- `POST /api/media`
- `GET /api/media/{id}`
- `DELETE /api/media/{id}`
- `POST /api/media/{id}/moderation-flags`
- `GET /api/gifs/search`
- `GET /api/gifs/trending`
- `POST /api/gifs/{mediaId}/recent`
- `GET /api/gifs/recent`

Implementation tasks:

- Add upload storage abstraction.
- Add request/response DTOs for media metadata.
- Add GIF search/recent/trending services.
- Add moderation workflows for media review.
- Add size/type validation and tests.
