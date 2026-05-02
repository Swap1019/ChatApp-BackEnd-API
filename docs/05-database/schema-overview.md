# Schema Overview

Status: domain schema implemented through EF Core.

The current schema is defined by `ChatApp.Infrastructure/DbContext/AppDbContext.cs`
and the migrations under `ChatApp.API/Migrations`.

Major areas:

- Identity: users, sessions, tokens, roles, permissions, privacy, suspensions
- Messaging: conversations, messages, threads, reactions, reads, deletions, pins
- Channels: channels, subscribers, admins, bans, URLs, pinned messages
- Media: media, metadata, avatars, GIF metadata, moderation, sensitive flags
- Social: contacts, blocks, posts, likes, stories, story views
