# Messaging Policies & Rules

## Overview
This document defines the business rules and policies that govern messaging in ChatApp.

---

## Message Creation Rules

### 1. **Access Control**
- User can only send messages to conversations they're a member of
- Cannot send if banned from conversation
- Cannot send if conversation is deleted (soft-deleted conversations are inaccessible)
- Cannot send if user is suspended or banned globally

### 2. **Content Validation**
```csharp
// At least one of:
✓ Text content (non-empty)
✓ Sticker
✓ GIF
✓ Media attachment

// Cannot send:
✗ Completely empty message
✗ Only whitespace
✗ All nulls
```

### 3. **Admin-Only Mode**
```csharp
if (conversation.OnlyAdminsCanSendMessage)
{
    // Only admins can send
    if (!conversationUser.IsAdmin)
        throw new UnauthorizedException("Only admins can send messages");
}
```

### 4. **Conversation Member Verification**
```csharp
var member = await _repo.GetConversationUser(conversationId, userId);
if (member == null)
    throw new AccessDeniedException("User is not a member of this conversation");
```

---

## Message Modification Rules

### 1. **Edit Rules**
- User can only edit their own messages
- Cannot edit deleted messages
- Cannot edit if message is older than 24 hours (configurable)
- Edited timestamp must be updated
- Editing updates the sequence number or timestamp for sorting

### 2. **Deletion Rules**
```csharp
public enum MessageDeletionScope
{
    SelfOnly,        // User's view only (soft delete for them)
    AllRecipients    // Everyone in conversation sees it as deleted
}
```

**Deletion Types**:
- **Self Delete**: User deletes from their view (not actually deleted)
- **Message Recall**: Message removed from everyone (within 5 min of send)
- **Hard Delete**: Admin removes completely (rare, for illegal content)

**Deletion Logic**:
```csharp
if (message.CreatedAt < DateTime.UtcNow.AddMinutes(-5))
{
    // After 5 minutes, can only soft-delete
    scope = MessageDeletionScope.SelfOnly;
}

// If admin and critical content:
if (userIsAdmin && isCritical)
{
    scope = MessageDeletionScope.AllRecipients;
}
```

---

## Message Reactions Policy

### 1. **Allowed Reactions**
- Standard Unicode emojis only
- No custom images
- No text strings
- Max 1 reaction per user per emoji

### 2. **Reaction Limits**
```
Max emojis per message: 20
Max users per emoji: 100
Max reactions per user: 50 (rate limiting)
```

### 3. **Reaction Validation**
```csharp
// Validate emoji is standard
if (!IsValidUnicodeEmoji(emoji))
    throw new InvalidOperationException("Invalid emoji");

// Check if user already reacted with this emoji
var existing = await _repo.GetReaction(messageId, userId, emoji);
if (existing != null)
{
    // Toggling: if exists, remove
    await _repo.DeleteReaction(existing);
}
else
{
    // Add new reaction
    var reaction = new MessageReaction { ... };
    await _repo.AddReaction(reaction);
}
```

---

## Message Threading Policy

### 1. **Thread Hierarchy**
```
Root Message (IsThreadRoot = true)
├── Reply 1
│   └── Sub-reply 1
├── Reply 2
└── Reply 3
```

### 2. **Threading Rules**
- Can reply to any message in a thread
- Replies update the parent thread timestamp
- Thread sorting: by root message creation, then reply creation
- Max thread depth: unlimited (but UI might limit display)

### 3. **Thread Notifications**
- New reply in thread notifies thread creator
- New reply notifies all participants
- Can mute thread notifications per user

---

## Message Read Receipts

### 1. **Read Status Tracking**
```csharp
public class MessageRead
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }
}
```

### 2. **When is Message Marked Read?**
- Option A: User opens message (immediate)
- Option B: User is in conversation for 2 seconds on message
- Option C: User explicitly marks as read

**Chosen: Option B (2-second visibility)**
- More reliable than Option A
- Less invasive than Option C
- Prevents fake read receipts

### 3. **Read Receipt Privacy**
```csharp
if (userPrivacy.ReadReceiptsVisibility == PrivacyLevel.Private)
{
    // Don't send read receipts to others
    return new { Unread = true };
}
```

---

## Message Forwarding Policy

### 1. **Forwarding Rules**
```csharp
public Guid? ForwardedFromMessageId { get; set; }

✓ Can forward to different conversation
✓ Can forward message + new text
✗ Cannot forward if original is deleted
✗ Cannot forward if no longer have access to original
```

### 2. **Attribution**
- Display "Forwarded from [User] in [Conversation]"
- Link to original message (if user has access)
- Show forward count

---

## Message Search & Filtering

### 1. **Searchable Content**
```csharp
// Full-text search enabled on:
✓ Message.Content (main text)
✓ GifMetadata.Title + Tags
✓ StickerPack.Name + Description

// Not searchable:
✗ Sticker.Emoji (too specific, use filter instead)
✗ Media attachment content (scan only filename)
```

### 2. **Search Filters**
```
- From: (user)
- Date: (range)
- Type: (text, sticker, gif, media)
- HasReaction: (specific emoji)
- InThread: (yes/no)
```

---

## Message Retention Policy

### 1. **Data Retention**
```
Active Conversations: Keep all messages indefinitely
Deleted Conversations: Keep for 30 days, then purge
Deleted Messages: Keep for 30 days (soft-delete), then hard-delete
```

### 2. **User Data Deletion (GDPR)**
```csharp
// When user requests account deletion:
1. Soft-delete all user content (messages, posts, etc)
2. Anonymize user references (username → "Deleted User")
3. Keep metadata for moderation (30 days)
4. Hard-delete after 30 days
```

---

## Message Sequence & Ordering

### 1. **Sequence Number**
```csharp
public long Sequence { get; set; }  // Per-conversation, monotonically increasing
```

**Why Sequence?**
- Guaranteed ordering even with clock skew
- Efficient gap detection
- Supports offline sync

**Generation**:
```csharp
var lastMessage = await _repo.GetLastMessageInConversation(conversationId);
var newSequence = (lastMessage?.Sequence ?? 0) + 1;
```

### 2. **Ordering Rules**
- Primary: Sequence number (ascending)
- Secondary: CreatedAt timestamp
- Threads: By root message sequence, then reply sequence

---

## Anti-Spam & Rate Limiting

### 1. **Message Rate Limits**
```
Per User:
- 10 messages per 10 seconds (burst)
- 60 messages per minute
- 500 messages per hour
- 5000 messages per day
```

### 2. **Detection**
```csharp
// Check rate limit
var messageCount = await _repo.CountRecentMessages(userId, minutes: 1);
if (messageCount >= 60)
    throw new RateLimitException("Too many messages");
```

### 3. **Spam Patterns**
- Duplicate messages within 30 seconds
- Identical message to multiple users in short time
- Excessive mentions (>5 per message)
- All caps only (>10 consecutive messages)

### 4. **Spam Consequences**
- First violation: Warning
- Second violation: Temporary mute (10 mins)
- Third violation: Temporary ban (1 hour)
- Repeated: Account suspension

---

## Mention & Notification System

### 1. **Mention Syntax**
```
@username   - Mention specific user
@all        - Mention all in conversation (admins only)
@here       - Mention online users only
```

### 2. **Mention Rules**
```
✓ Max 10 mentions per message
✗ Cannot mention user not in conversation
✗ Cannot mention if blocked by target
✗ Cannot mention if user has muted notifications
```

### 3. **Notification Behavior**
```
User is notified if:
✓ Directly mentioned (@user)
✓ Replying to their message
✓ Reaction on their message
✓ New message in conversation they're in

User NOT notified if:
✗ Muted conversation
✗ Mentioned as @all but turned off @all notifications
✗ Blocked the sender
✗ Do Not Disturb mode enabled
```

---

## Sensitive Content Handling

### 1. **Sensitive Content Detection**
```csharp
public class SensitiveContentFlag
{
    public Guid Id { get; set; }
    public Guid MediaId { get; set; }
    
    public SensitiveContentType Type { get; set; }  // Violence, Adult, etc.
    public SensitiveContentSeverity Severity { get; set; }  // Low, Medium, High
    
    public bool RequiresWarning { get; set; }
    public bool AutoBlur { get; set; }
}
```

### 2. **Content Warning Display**
- Show warning before displaying sensitive content
- Allow user to view or hide
- Track view statistics (for content moderation)

### 3. **User Preferences**
```csharp
public UserPrivacy
{
    public bool ShowSensitiveContent { get; set; }
    public bool AutoBlurSensitiveContent { get; set; }
    public bool NotifyOfSensitiveContent { get; set; }
}
```

---

## Message Archival

### 1. **Archive Rules**
- Users can archive conversations (removes from list, keeps history)
- Archived conversations still searchable
- Conversations auto-archive if no activity for 6 months

### 2. **Archive Metadata**
```csharp
public class ConversationArchive
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ArchivedAt { get; set; }
    public string? ArchivedReason { get; set; }
    public bool IsHidden { get; set; }  // Hidden vs just archived
}
```

---

## Compliance & Audit

### 1. **Message Audit Trail**
Every modification tracked:
- Creation timestamp & user
- Edit timestamp & user
- Delete timestamp & user
- Who reported message

### 2. **Admin Visibility**
- Admins can see all messages (even deleted)
- Admins can view full audit trail
- Admins can restore deleted messages (within 30 days)

### 3. **Regulatory Compliance**
- GDPR: Can export user messages
- Data retention: Follow configured policies
- Encryption: E2E encryption planned for sensitive messages

