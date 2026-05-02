# Architecture Decision Record

## Clean Architecture & Domain-Driven Design

### Architectural Layers

```
┌─────────────────────────────────────────────┐
│         API Controllers (Presentation)      │
├─────────────────────────────────────────────┤
│     Application Services & DTOs (DTOs)      │
├─────────────────────────────────────────────┤
│    Domain Entities & Business Logic (DDD)   │
├─────────────────────────────────────────────┤
│  Infrastructure (DB, Repositories, Services)│
└─────────────────────────────────────────────┘
```

### Layer Responsibilities

#### 1. **Domain Layer** (ChatApp.Domain)
- **Purpose**: Core business logic
- **Contains**: 
  - Entities (Aggregates)
  - Value Objects
  - Enumerations
  - Business rules validation
- **Dependencies**: None (most isolated)
- **Key Principle**: No external dependencies, only .NET Base Class Library

#### 2. **Application Layer** (ChatApp.Application)
- **Purpose**: Use cases & application orchestration
- **Contains**:
  - Service interfaces
  - DTOs for API communication
  - Query/Command handlers
  - Authorization policies
- **Dependencies**: Domain layer
- **Key Principle**: Translates domain logic to application logic

#### 3. **Infrastructure Layer** (ChatApp.Infrastructure)
- **Purpose**: Technical implementation
- **Contains**:
  - DbContext (EF Core)
  - Repository implementations
  - Database migrations
  - External service integrations
  - Data persistence
- **Dependencies**: Domain + Application
- **Key Principle**: Implements abstractions defined in Application layer

#### 4. **API/Presentation Layer** (ChatApp.API)
- **Purpose**: HTTP endpoints & request handling
- **Contains**:
  - Controllers
  - Dependency injection setup
  - Middleware configuration
  - Request/response mapping
- **Dependencies**: All layers (orchestrates them)
- **Key Principle**: Thin controllers, delegates to services

#### 5. **Common Layer** (ChatApp.Common)
- **Purpose**: Shared utilities & shared DTOs
- **Contains**:
  - Shared enumerations
  - Common helpers
  - Shared response types
- **Dependencies**: None (lowest level utilities)

---

## Data Flow

### Create Message Flow

```
1. POST /api/messages
   ↓
2. Controller receives CreateMessageDto
   ↓
3. Controller validates & calls MessageService
   ↓
4. MessageService:
   - Applies business rules (user permissions, conversation access)
   - Validates message content
   - Calls repository to persist
   ↓
5. Repository (using EF Core):
   - Adds Message entity to DbSet
   - Saves to PostgreSQL
   ↓
6. Returns created message to API
   ↓
7. Controller transforms to MessageResponseDto
   ↓
8. Returns 201 Created response
```

---

## Entity Design Patterns

### Aggregate Pattern
Each entity has a clear boundary:

**User Aggregate** (Root)
- User (aggregate root)
- UserToken (child)
- UserSession (child)
- UserSuspension (child)
- All managed through User repository

**Conversation Aggregate** (Root)
- Conversation (aggregate root)
- ConversationUser (child)
- ConversationUserAdmin (child)
- ConversationUserBan (child)
- Messages (child)
- All managed through Conversation repository

---

## Dependency Injection Strategy

### Service Registration Pattern

```csharp
services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(...));

services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IMessageService, MessageService>();
services.AddScoped<IAuthorizationService, AuthorizationService>();
```

### Lifetime Scopes
- **Singleton**: Configuration, logging infrastructure
- **Scoped**: DbContext (one per request), services that use DbContext
- **Transient**: Stateless utilities

---

## RBAC (Role-Based Access Control)

### Permission Model

```
User
  ↓
[Many-to-Many through UserRole]
  ↓
Role
  ↓
[Many-to-Many through RolePermission]
  ↓
Permission
```

### Example Permissions
- `create_message`
- `delete_message_own` (own messages only)
- `delete_message_any` (any message)
- `manage_conversation`
- `ban_user`
- `suspend_user` (admin only)

---

## Key Database Design Decisions

### 1. UUID vs Auto-Increment
**Decision**: UUID (Guid in .NET)
**Rationale**:
- Can generate IDs on client
- No conflicts in distributed systems
- Privacy (IDs don't reveal sequence)

### 2. Soft Deletes Strategy
**Decision**: All entities have IsDeleted flag
**Implementation**:
- Global query filters hide deleted items
- Can restore deleted data
- Maintains referential integrity

### 3. Audit Trail Pattern
**Decision**: Created/Updated fields on entities
**Benefit**:
- Track entity lifecycle
- Support for undo operations
- Compliance & auditing

### 4. Composite Keys for Relationships
**Decision**: Use (FK1, FK2) as PK for linking tables
**Example**: 
```sql
UserRole {
  PRIMARY KEY (UserId, RoleId)
}
```
**Benefit**: Prevents duplicate associations, efficient queries

---

## API Response Structure

### Standard Response Envelope

```json
{
  "success": true,
  "data": { /* actual data */ },
  "message": "Operation successful",
  "timestamp": "2026-05-02T10:30:00Z"
}
```

### Error Response

```json
{
  "success": false,
  "error": {
    "code": "UNAUTHORIZED",
    "message": "User is not authorized to perform this action"
  },
  "timestamp": "2026-05-02T10:30:00Z"
}
```

---

## Concurrency Handling

### Optimistic Concurrency (Future)
- Use `ConcurrencyStamp` on User entity
- EF Core checks version before update
- Returns 409 Conflict if data changed

### Pessimistic Locking (for Critical Operations)
- Use database transactions
- Lock rows during critical updates

---

## Scalability Considerations

### Caching Strategy (Planned)
- Cache frequently accessed data (roles, permissions)
- Invalidate on updates
- Use Redis for distributed caching

### Database Partitioning (Future)
- Partition Messages by ConversationId
- Partition UserRecentGif by UserId
- Improves query performance on large tables

### Read Replicas
- ReadOnly connection for reports
- Separate analytics queries from transactional queries
