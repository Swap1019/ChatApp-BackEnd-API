# ChatApp Backend API

A robust .NET-based REST API for a real-time communication platform. Provides messaging, conversation management, media sharing, social features, and an admin panel with granular permission management.

This project continues the evolution of ChatApp, originally built with Django as an MVP.

🔗 Previous Django Backend (MVP):
https://github.com/Swap1019/ChatApp-BackEnd

The Django version served as a prototype to validate core features and architecture decisions.  
This .NET implementation is a complete rewrite focused on scalability, performance, and production readiness.


## 🌟 Key Features

### Core Messaging
- **Real-time Conversations** - Direct 1-to-1 and group chats
- **Message Threading** - Organize conversations with threaded replies
- **Message Reactions** - React to messages with emojis
- **Message Pinning** - Pin important messages for quick reference
- **Message Deletion & Editing** - Edit sent messages or delete them with tracking
- **Message Read Receipts** - See when messages are read
- **Media Attachments** - Share images and files in conversations
- **Message Forwarding** - Forward messages to other conversations

### Conversation Management
- **Shareable URLs** - Generate unique, customizable URLs for conversations
- **URL Rotation** - Archive old URLs for security
- **Admin Controls** - Restrict messaging to admins only
- **Member Management** - Add, remove, and manage conversation members
- **User Bans** - Ban users from conversations with optional expiration
- **Conversation Archiving** - Soft-delete conversations while preserving data

### User Management
- **Account Security**:
  - Email & Phone verification
  - Password management with secure hashing
  - Failed login attempt tracking with auto-lockout
  - Session management
  - Security tokens and authentication

- **User Presence**:
  - Online/offline status tracking
  - Last active timestamp
  - Activity monitoring

- **Account Status**:
  - Active, Suspended, or Banned states
  - Suspension history tracking
  - Admin oversight

### Social Features
- **Contacts** - Manage friend/contact lists
- **Block Users** - Block other users and view who blocked you
- **Posts & Stories** - Share posts with likes
- **Instagram-style Stories** - Time-limited content
- **Post Reactions** - Like posts

### Admin & Moderation
- **Role-Based Access Control (RBAC)** - Permission system
- **Granular Permissions** - 26+ customizable permissions
- **Dynamic Role Management** - Create and modify roles at runtime
- **Content Moderation** - Flag and review sensitive content
- **User Suspension & Bans** - Manage user access
- **Audit Logging** - Track all admin actions

### Media Management
- **Media Upload** - Store and manage media files
- **Content Moderation** - Flag inappropriate content
- **Sensitive Content Detection** - Mark explicit content
- **Media Deduplication** - Using content hashing

### Notifications
- **Real-time Notifications** - Instant updates for user actions
- **Priority Levels** - High/Medium/Low priority notifications
- **Notification Grouping** - Consolidate similar notifications
- **Mark as Read** - Track notification status

## 🏗️ Architecture

### Project Structure

```
ChatApp.Backend/
├── ChatApp.Domain/          # Domain entities and business logic
├── ChatApp.Infrastructure/  # Database, services, and repositories
├── ChatApp.Application/     # Business services and DTOs
├── ChatApp.API/             # REST API controllers and endpoints
└── ChatApp.Common/          # Shared utilities and enums
```

### Technology Stack

- **.NET 10.0** - Latest .NET framework
- **Entity Framework Core** - ORM for database access
- **PostgreSQL** - Cloud-ready relational database
- **JWT Authentication** - Secure token-based auth
- **Dependency Injection** - Built-in IoC container
- **RESTful API** - Standard REST conventions

## 📊 Database Schema Overview

### Core Entities

#### Identity Management
- **User** - User profiles with authentication and presence tracking
- **UserRole** - Many-to-many user-role assignments
- **Permission** - Granular permission definitions
- **Role** - Role definitions with permissions
- **RolePermission** - Many-to-many role-permission assignments
- **UserToken** - Authentication tokens
- **UserSession** - Active user sessions
- **UserSuspension** - Track user account suspensions

#### Messaging
- **Conversation** - 1-to-1 or group conversations
- **ConversationUser** - Members in conversations with admin flags
- **ConversationUrl** - Shareable URLs for conversations (with history)
- **ConversationUserBan** - User bans from conversations
- **ConversationUserAdmin** - Admin permissions within conversations
- **Message** - Individual messages with threading support
- **MessageThread** - Root message for threaded conversations
- **MessageAttachment** - File uploads in messages
- **MessageReaction** - Emoji reactions to messages
- **MessageRead** - Track when messages are read
- **MessageDeletion** - Track deleted messages
- **PinnedMessage** - Important messages pinned to conversations

#### Social Features
- **Contact** - User contact lists
- **BlockedUser** - Block relationships between users
- **Post** - User posts for feeds
- **PostLike** - Likes on posts
- **PostAttachment** - Media in posts
- **Story** - Time-limited content
- **StorySeen** - Track who viewed stories

#### Media & Notifications
- **Media** - File storage metadata with content hashing
- **MediaModeration** - Content moderation flags
- **SensitiveContentFlag** - Mark explicit content
- **Notification** - User notifications with priorities

## 🔐 Authentication & Authorization

### Permission System

The app uses a **Django-inspired permission system** with complete customization:

**26 Default Permissions across categories:**
- User Management (view, suspend, ban, delete, edit)
- Conversation Management (view, delete, moderate, archive)
- Content Moderation (flag, review, delete messages)
- Role Management (view, create, edit, delete, assign)
- System Management (access, settings, logs)
- Security (API keys, audit logs)

**Built-in Roles:**
- **Administrator** - Full system access
- **Moderator** - Content review and moderation
- **User** - Standard user permissions

**Customization:**
Permissions are fully editable via `role-permissions-config.json`:

```json
{
  "roles": [
    {
      "name": "Senior Admin",
      "permissions": ["manage.users.delete", "manage.system.settings", ...]
    },
    {
      "name": "Junior Admin",
      "permissions": ["manage.users.view", "view.reports"]
    }
  ]
}
```

## 📋 Entity Relationships

### Key Relationships

**One-to-Many:**
- User → Messages (messages sent by user)
- User → Conversations (conversations created by user)
- Conversation → Messages (all messages in conversation)
- Conversation → PinnedMessages (pinned messages in conversation)

**Many-to-Many (with join tables):**
- User ↔ Conversation (via ConversationUser)
- User ↔ Role (via UserRole)
- Role ↔ Permission (via RolePermission)

**Self-Referential:**
- User → BlockedUsers (relationships between users)
- Message → MessageThread (threaded replies)

**Query Filters:**
- Soft-deleted conversations are excluded by default
- Soft-deleted messages are excluded by default
- Suspended users are tracked but queryable

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL (local or cloud)
- Git

### Backend Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd ChatApp/BackEnd/backend
   ```

2. **Install dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure database:**
   - Update connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ChatAppDb;User Id=sa;Password=YourPassword;"
     }
   }
   ```

4. **Run migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Seed default data (optional):**
   This seeds default permissions and roles from `role-permissions-config.json`
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

6. **Run the API:**
   ```bash
   dotnet run
   ```

   API will be available at `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`
   - Health check: `https://localhost:5001/health`

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Authenticate user
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/logout` - Logout user

### Users
- `GET /api/users/{id}` - Get user profile
- `PUT /api/users/{id}` - Update user profile
- `PUT /api/users/{id}/password` - Change password
- `PUT /api/users/{id}/email` - Change email
- `PUT /api/users/{id}/phone` - Change phone number
- `GET /api/users/{id}/roles` - Get user roles
- `POST /api/users/{id}/roles` - Assign role to user

### Conversations
- `POST /api/conversations` - Create conversation
- `GET /api/conversations` - List user conversations
- `GET /api/conversations/{id}` - Get conversation details
- `PUT /api/conversations/{id}` - Update conversation
- `DELETE /api/conversations/{id}` - Delete conversation
- `POST /api/conversations/{id}/members` - Add member
- `DELETE /api/conversations/{id}/members/{userId}` - Remove member
- `GET /api/conversations/{id}/bans` - List banned users
- `POST /api/conversations/{id}/bans` - Ban user
- `DELETE /api/conversations/{id}/bans/{userId}` - Unban user
- `POST /api/conversations/{id}/url` - Generate shareable URL
- `PUT /api/conversations/{id}/url` - Rotate URL

### Messages
- `POST /api/conversations/{conversationId}/messages` - Send message
- `GET /api/conversations/{conversationId}/messages` - List messages
- `PUT /api/messages/{id}` - Edit message
- `DELETE /api/messages/{id}` - Delete message
- `POST /api/messages/{id}/reactions` - Add reaction
- `DELETE /api/messages/{id}/reactions/{emoji}` - Remove reaction
- `POST /api/messages/{id}/read` - Mark as read
- `POST /api/conversations/{conversationId}/messages/{id}/pin` - Pin message
- `DELETE /api/conversations/{conversationId}/messages/{id}/pin` - Unpin message

### Admin & Roles
- `GET /api/admin/roles` - List all roles
- `POST /api/admin/roles` - Create role
- `PUT /api/admin/roles/{id}` - Update role
- `DELETE /api/admin/roles/{id}` - Delete role
- `GET /api/admin/permissions` - List all permissions
- `PUT /api/admin/roles/{id}/permissions` - Update role permissions
- `GET /api/admin/users` - List all users (admin only)
- `POST /api/admin/users/{id}/suspend` - Suspend user
- `POST /api/admin/users/{id}/unsuspend` - Unsuspend user

### Notifications
- `GET /api/notifications` - List user notifications
- `PUT /api/notifications/{id}/read` - Mark notification as read
- `DELETE /api/notifications/{id}` - Delete notification

### Message Sequencing
Messages use monotonically increasing `Sequence` numbers per conversation for reliable ordering without timestamp dependencies.

### Soft Deletes
Records are soft-deleted (marked as deleted, not physically removed) for data integrity:
- `IsDeleted` boolean flag
- `DeletedAt` timestamp
- Automatic query filters exclude deleted records

### Transaction Support
Complex operations use transactions to maintain data consistency.

## ⚙️ Configuration

### Permission Customization

Edit `/backend/ChatApp.Infrastructure/Seeding/role-permissions-config.json` to:
- Add new permissions
- Modify role assignments
- Create custom roles
- Adjust security levels per environment

### Database Configuration

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=ChatAppDb;..."
  }
}
```

## 🔄 Key Workflows

### Create a Conversation

```csharp
var conversation = new Conversation("Project Discussion", isGroup: true);
conversation.CreatedById = currentUserId;
await dbContext.Conversations.AddAsync(conversation);
await dbContext.SaveChangesAsync();
```

### Send a Message

```csharp
var message = new Message 
{
    ConversationId = conversationId,
    SenderId = currentUserId,
    Content = "Hello team!",
    Sequence = nextSequenceNumber
};
await dbContext.Messages.AddAsync(message);
await dbContext.SaveChangesAsync();
```

### Generate Shareable URL

```csharp
conversation.GenerateUrlSlug("project-meeting-2024");
var urlEntry = new ConversationUrl(conversation.Id, "project-meeting-2024");
```

### Check User Permissions

```csharp
var role = await dbContext.Roles.Include(r => r.Permissions)
    .FirstAsync(r => r.Name == "Moderator");

if (role.HasPermission("moderate.messages.delete"))
{
    // Allow message deletion
}
```

## 🧪 Testing

Run unit tests:
```bash
dotnet test
```

Run with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

## 📖 Documentation

- **Permission Management**: See `ChatApp.Infrastructure/Seeding/PERMISSION_CUSTOMIZATION_GUIDE.md` for detailed permission system documentation
- **API Documentation**: Available at `/swagger` endpoint when running in development mode
- **Database Schema**: See `ChatApp.Infrastructure/DbContext/AppDbContext.cs` for entity models and relationships

### Coding Standards
- Follow C# naming conventions (PascalCase for public members)
- Use async/await for I/O operations
- Write unit tests for new features
- Document public methods with XML comments

## 📄 License

This project is licensed under the MIT License - see LICENSE file for details.

## 🐛 Troubleshooting

### Database Connection Issues
```
Error: Connection timeout or refused
```
- Verify PostgreSQL is running
- Check connection string in `appsettings.json`
- Ensure database user has proper permissions
- Verify port 5432 is accessible

### API Won't Start
```
Error: Port 5001 already in use
```
- Check what's using port 5001: `netstat -ano | findstr :5001`
- Kill the process or change port in `launchSettings.json`
- Verify .NET 10.0 SDK: `dotnet --version`

### Migration Fails
```
Error: Failed to apply migration
```
- Ensure database exists and is accessible
- Run `dotnet ef migrations add <Name>` first
- Check for pending migrations: `dotnet ef migrations list`
- Rollback if needed: `dotnet ef database update <PreviousMigration>`

### JWT Token Issues
```
Error: Invalid token or token expired
```
- Verify JWT secret in `appsettings.json`
- Check token expiration times
- Refresh token if expired
- Ensure Authorization header format: `Bearer <token>`

## 📊 Performance Considerations

- Messages use monotonically increasing Sequence numbers for reliable ordering
- Eager loading used strategically to avoid N+1 queries
- Indexes on frequently queried columns (UserId, ConversationId, CreatedAt)
- Soft deletes with query filters to maintain data integrity
- Connection pooling configured for database efficiency

## 🔐 Security

- JWT authentication with configurable token expiration
- Password hashing using industry-standard algorithms
- Rate limiting on authentication endpoints
- HTTPS enforced in production
- CORS configured for frontend domains
- SQL injection protection via parameterized queries (EF Core)
- Authorization checks on all protected endpoints

## 📞 Support

For issues and feature requests, please:
1. Check existing GitHub issues
2. Review the troubleshooting section
3. Open a new issue with detailed information
4. Include API logs and error messages

## 🗺️ Roadmap

- [ ] SignalR for WebSocket real-time messaging
- [ ] Message search with full-text indexing
- [ ] End-to-end encryption
- [ ] Advanced caching strategy (Redis)
- [ ] API rate limiting and throttling
- [ ] GraphQL API alternative
- [ ] Database replication for high availability
- [ ] Event sourcing for audit trail
- [ ] Media processing pipeline
- [ ] Analytics and metrics dashboard

---

**Built with ❤️ for seamless backend communication**
