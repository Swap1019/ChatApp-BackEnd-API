# Authorization & Access Control Policies

## Overview
This document defines the authorization model, roles, and permissions in ChatApp.

---

## Authentication

### 1. **Authentication Methods**
- [ ] **Email + Password** (current)
  - User registers with email/phone/username
  - Password hashed with bcrypt (min 12 rounds)
  - MFA optional but recommended

- [ ] **Session-Based** (current)
  - Create UserSession on login
  - JWT or session token validation
  - Automatic logout after 30 days of inactivity

- [ ] **Multi-Factor Authentication** (future)
  - SMS-based OTP (One-Time Password)
  - TOTP (Time-based OTP) apps
  - Email verification

### 2. **Password Policy**
```
Minimum Length: 8 characters
Must contain:
  ✓ Uppercase letter (A-Z)
  ✓ Lowercase letter (a-z)
  ✓ Number (0-9)
  ✓ Special character (!@#$%^&*)

Cannot:
  ✗ Contain username
  ✗ Be used previously (10 history)
```

### 3. **Account Lockout**
```
After 5 failed login attempts:
- Account locks for 15 minutes
- User receives email notification
- Can unlock via email link
```

---

## Authorization Model: RBAC (Role-Based Access Control)

### 1. **Role Hierarchy**

```
SuperAdmin (UID: 1)
  ├── Full platform control
  ├── Create/manage admins
  └── Access all features

SystemAdmin (UID: 2)
  ├── System configuration
  ├── Manage infrastructure
  └── View audit logs

Moderator (UID: 3)
  ├── Review user reports
  ├── Suspend/ban users
  ├── Delete content
  └── Cannot modify system config

ContentModerator (UID: 4)
  ├── Review flagged media
  ├── Mark NSFW content
  ├── Delete inappropriate content
  └── Limited user management

SupportAgent (UID: 5)
  ├── View user tickets
  ├── Help users
  └── Limited account access

User (UID: 6) - Default role for all users
  ├── Send messages
  ├── Create conversations
  └── Standard features
```

### 2. **Permission Structure**

```csharp
public enum PermissionCodename
{
    // User Management
    ManageUsers,
    SuspendUser,
    BanUser,
    ViewUserDetails,
    
    // Content Moderation
    DeleteMessage,
    DeletePost,
    FlagContent,
    ReviewReports,
    
    // Conversation Management
    CreateConversation,
    DeleteConversation,
    ManageConversationAdmins,
    BanUserFromConversation,
    
    // Channel Management
    CreateChannel,
    DeleteChannel,
    ArchiveChannel,
    
    // System Administration
    ViewAuditLogs,
    ManageRoles,
    UpdateSystemSettings,
    ViewAnalytics,
    
    // Bot Management
    CreateBot,
    ManageBots,
    ApproveBot,
    
    // Support
    ViewTickets,
    RespondToTickets,
    EscalateTicket,
}
```

### 3. **Permission-Role Assignment**

```
┌─────────────────┬──────────────┬──────────────┬────────────────────┐
│ Permission      │ SuperAdmin   │ Moderator    │ User               │
├─────────────────┼──────────────┼──────────────┼────────────────────┤
│ DeleteMessage   │ ✓            │ ✓            │ ✓ (own only)       │
│ SuspendUser     │ ✓            │ ✓            │ ✗                  │
│ ManageRoles     │ ✓            │ ✗            │ ✗                  │
│ CreateMessage   │ ✓            │ ✓            │ ✓ (own)            │
│ ViewAuditLogs   │ ✓            │ ✗            │ ✗                  │
└─────────────────┴──────────────┴──────────────┴────────────────────┘
```

---

## Conversation-Level Permissions

### 1. **Conversation Roles**
```csharp
public class ConversationUser
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    
    public bool IsAdmin { get; set; }          // Conversation admin
    public DateTime JoinedAt { get; set; }
    public bool IsMuted { get; set; }          // Mute notifications
}
```

### 2. **Admin Permissions in Conversation**
```
Conversation Admins can:
✓ Delete messages
✓ Ban users from conversation
✓ Promote/demote other admins
✓ Change conversation settings
✓ Pin messages
✗ Cannot delete conversation (creator or super-admin only)
```

### 3. **Access Control**
```csharp
// Check if user can access conversation
if (!await _repo.IsMemberOf(userId, conversationId))
    throw new AccessDeniedException("Not a member");

// Check if user is banned
if (await _repo.IsBanned(userId, conversationId))
    throw new AccessDeniedException("User is banned");

// Check if conversation is deleted
if (conversation.IsDeleted && !userIsAdmin)
    throw new AccessDeniedException("Conversation is deleted");
```

---

## Channel-Level Permissions

### 1. **Channel Roles**
```
Channel Creator
  ├── Full control
  └── Can delete channel

Channel Admin (appointed by creator)
  ├── Manage members
  ├── Delete messages
  ├── Pin messages
  └── Cannot delete channel

Channel Member
  ├── View messages
  ├── Send messages
  └── React to messages
  
Non-Member (if public channel)
  ├── View read-only
  └── Cannot send messages
```

### 2. **Channel Visibility**
```
Public Channel:
- Discoverable in search
- Anyone can join
- Non-members can view if link shared

Private Channel:
- Not discoverable
- Requires access code or invite
- Only members can view
- Access code expires after 30 days
```

### 3. **Permission Precedence**
```
SuperAdmin > ChannelCreator > ChannelAdmin > Member > PublicView
```

---

## Resource-Level Permissions

### 1. **Message Permissions**

```
User can delete message if:
✓ They are the sender AND within 5 minutes
✓ They are conversation admin
✓ They are platform admin

User can edit message if:
✓ They are the sender AND within 24 hours
✗ Message is already deleted
```

### 2. **Media Permissions**

```
User can delete media if:
✓ They uploaded it
✓ They are admin
✓ Media is flagged for removal

User can view media if:
✓ They have access to conversation/channel with media
✓ Media is not age-restricted
✓ Media is not private
```

### 3. **Privacy-Aware Access**

```csharp
// Check if user can see another user's profile
if (targetUser.ProfilePrivacy == PrivacyLevel.Private)
{
    // Only user themselves and admins can see
    if (requestingUserId != targetUser.Id && !isAdmin)
        throw new AccessDeniedException("Profile is private");
}

// Check exceptions
if (await _repo.HasPrivacyException(targetUser.Id, requestingUserId))
{
    // Specific exception might override privacy
    return profileData;
}
```

---

## Permission Checking Pattern

### 1. **Authorization Handler** (Centralized)
```csharp
public class DeleteMessageAuthorizationHandler : IAuthorizationHandler
{
    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        var requirement = context.Requirements.OfType<DeleteMessageRequirement>().FirstOrDefault();
        if (requirement == null) return;
        
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var messageId = context.Resource as Guid?;
        
        var message = await _repo.GetMessage(messageId.Value);
        
        // Check if user is sender
        if (message.SenderId == Guid.Parse(userId))
        {
            // Can only delete within 5 minutes
            if (DateTime.UtcNow - message.CreatedAt <= TimeSpan.FromMinutes(5))
            {
                context.Succeed(requirement);
                return;
            }
        }
        
        // Check if user is admin
        if (await _authService.IsAdmin(userId))
        {
            context.Succeed(requirement);
            return;
        }
    }
}
```

### 2. **Using Authorization in API**
```csharp
[HttpDelete("messages/{messageId}")]
[Authorize]  // Must be authenticated
public async Task<IActionResult> DeleteMessage(Guid messageId)
{
    // Authorization handler checks permission
    var authResult = await _authorizationService.AuthorizeAsync(
        User, 
        messageId, 
        "DeleteMessage"
    );
    
    if (!authResult.Succeeded)
        return Forbid("You cannot delete this message");
    
    await _messageService.DeleteMessage(messageId);
    return Ok();
}
```

---

## Special Cases

### 1. **Content Creator Rights**
```
Content Creator can:
✓ Delete their own content
✓ Edit their own content
✓ See analytics on their content
✓ See who has engaged with their content
```

### 2. **User Privacy Rights**
```
User can:
✓ See all data about themselves
✓ Export their data (GDPR)
✓ Delete their data
✓ Control privacy settings
✓ Block other users
```

### 3. **Admin Override**
```
Platform Admin can:
✓ Perform any action for security/compliance
✓ Access deleted content
✓ See private conversations (with warrant)
✓ Override user privacy settings
✗ MUST log all overrides (audit trail)
```

---

## Session & Token Management

### 1. **Session Entity**
```csharp
public class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }      // Device info
    
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }   // 30 days
    public DateTime? RevokedAt { get; set; }   // User logout
    
    public bool IsActive => !RevokedAt.HasValue && 
                            (!ExpiresAt.HasValue || ExpiresAt > DateTime.UtcNow);
}
```

### 2. **Token Validation**
```csharp
// Before each request:
1. Extract token from header
2. Verify signature
3. Check expiration
4. Check if session is revoked
5. Extract user claims
6. Load user permissions

// If any check fails:
→ Return 401 Unauthorized
```

### 3. **Token Refresh**
```
Initial Login:
- Return access token (15 min expiry)
- Return refresh token (30 day expiry)

Refresh Request:
- Use refresh token to get new access token
- Don't need to re-authenticate
- Refresh token can only be used once

Logout:
- Revoke session
- Client deletes tokens locally
```

---

## Audit Trail

### 1. **Logged Actions**
```
Authorization-related actions:
✓ Login/Logout
✓ Permission granted/revoked
✓ Role assigned/removed
✓ Authorization failures (3 attempts = warning)
✓ Admin overrides
✗ Regular API calls (too much volume)
```

### 2. **Audit Log Entry**
```csharp
public class AuditLogEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; }         // "DELETE_MESSAGE", "SUSPEND_USER"
    public string ResourceType { get; set; }   // "Message", "User"
    public Guid? ResourceId { get; set; }
    public bool WasSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

---

## Future Enhancements

- [ ] **Attribute-Based Access Control (ABAC)**: More granular rules based on attributes
- [ ] **Fine-Grained Permissions**: Per-message, per-media permissions
- [ ] **Delegation**: Users can delegate permissions to others
- [ ] **Time-Based Permissions**: Permissions active only during certain times
- [ ] **Geographic Restrictions**: Block access from certain regions

