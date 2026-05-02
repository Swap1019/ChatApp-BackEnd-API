# Authentication API

Status: planned.

The current backend project does not yet contain authentication controllers,
JWT issuance, refresh-token endpoints, or ASP.NET Core authentication middleware.
The domain model includes users, sessions, tokens, roles, permissions, and
suspensions, so the data foundation is ready for the API layer.

Planned endpoints:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`

Implementation tasks:

- Add request/response DTOs.
- Add password hashing and credential validation services.
- Configure JWT bearer authentication in `Program.cs`.
- Add controllers or minimal API endpoints.
- Add validation, error responses, and integration tests.
