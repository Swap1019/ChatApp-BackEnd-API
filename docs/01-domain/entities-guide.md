# Entity Design Guide

## Overview
This document explains the complex entity designs and why they're structured the way they are.

---

## Identity Entities

### User
**Purpose**: Core user entity representing a person in the system.

**Key Fields**:
```csharp
public Guid Id { get; set; }
public string Username { get; set; }          // Unique identifier
public string Email { get; set; }              // Unique, for verification
public string PhoneNumber { get; set; }        // Unique, for verification
public string PasswordHash { get; set; }       // Never store plain password
public bool IsEmailVerified { get; set; }
public bool IsPhoneNumberVerified { get; set; }
```

**Why Composite Verification?**
- Email can be spoofed or typo'd
- Phone verification provides additional security
- Users might prefer one method over the other
- Helps prevent account takeover

**Suspension System**:
```csharp
public UserAccountStatus AccountStatus { get; set; }  // Active, Suspended, Banned
public ICollection<UserSuspension> Suspensions { get; set; }
```

**Why Separate Suspension Entity?**
- Track suspension history (who suspended, when, why)
- Support temporary vs permanent bans
- Maintain audit trail for compliance
- Can restore users after suspension

**Online Presence**:
```csharp
public OnlineStatus OnlineStatus { get; set; }  // Online, Away, Busy, Offline
public DateTime? LastActiveAt { get; set; }     // Last interaction time
```

**Why Multiple States?**
- Users can explicitly set "Do Not Disturb" / "Busy"
- Last active timestamp helps detect stale users
- Better UX: show "Away for 2 hours" vs just "Offline"

---

### UserPrivacy & UserPrivacyException
**Purpose**: Granular privacy control per user field.

**Design**:
```csharp
public class UserPrivacy
{
    public Guid UserId { get; set; }            // One-to-one
    public PrivacyLevel ProfileVisibility { get; set; }
    public PrivacyLevel ContactVisibility { get; set; }
    public PrivacyLevel OnlineStatusVisibility { get; set; }
    // ... more fields
}

public class UserPrivacyException
{
    public Guid OwnerUserId { get; set; }       // Whose privacy setting
    public Guid TargetUserId { get; set; }      // User affected by exception
    public PrivacyField Field { get; set; }     // Which field (Email, Phone, etc)
    public PrivacyLevel Exception { get; set; } // Override level
}
```

**Privacy Levels**:
- `Public`: Visible to everyone
- `Friends`: Visible only to contacts
- `Private`: Visible only to self

**Why This Design?**
1. **Separate entity for exceptions**: Can quickly determine if an exception exists
2. **Bidirectional control**: User can allow OR restrict specific people
3. **Field-level granularity**: Each data type can have different privacy
4. **No duplicate data**: Exceptions reference UserPrivacy implicitly

**Example Usage**:
```
User A sets Email to "Private"
User B is added to exception, Email visibility = "Public"
→ Only User B can see User A's email
```

---

## Messaging Entities

### Message
**Purpose**: Core messaging entity.

**Why Multiple Content Types?**
```csharp
public string Content { get; set; }                    // Text content
public Guid? StickerId { get; set; }                   // Optional sticker
public Guid? GifMediaId { get; set; }                  // Optional GIF
public ICollection<MessageAttachment> MessageAttachments { get; set; }  // Files
```

**Rationale**:
- Separation of concerns: message can have ANY combination of these
- Not mutually exclusive: can send text + sticker + GIF + media
- Flexible: easy to extend with new content types

**Message Threading**:
```csharp
public Guid? ThreadId { get; set; }            // Groups replies
public bool IsThreadRoot { get; set; }         // Root of thread
public Guid? RepliedMessageId { get; set; }    // Direct reply to
```

**Why Separate Thread from Direct Reply?**
- Can have nested replies without creating deep chains
- Threads keep conversation organized
- Direct reply enables "quote" functionality
- Both together = rich conversation structure

**Message Status Tracking**:
```csharp
public MessageStatus MessageStatus { get; set; }  // Sent, Delivered, Read
public ICollection<MessageRead> Reads { get; set; }  // Per-user read status
```

**Why Collection of Reads?**
- In group chats, need to track who read it
- Shows "2 people have read this" without storing full list
- Can query recent unread messages efficiently

**Message Reactions**:
```csharp
public ICollection<MessageReaction> Reactions { get; set; }
```

**Separate Entity Instead of List<string>?**
- Multiple users can react with same emoji
- Need to track which user added which reaction
- Can add/remove reactions efficiently
- Supports pagination of reactions

### UserRecentSticker & UserRecentGif
**Purpose**: Track user preferences for quick access.

**Why Separate from User?**
```csharp
// Instead of:
public User { 
    public List<Sticker> RecentStickers { get; set; }  // ❌ Bloats User
}

// We have:
public class UserRecentSticker {
    public Guid UserId { get; set; }
    public Guid StickerId { get; set; }
    public DateTime LastUsedAt { get; set; }
    public int UsageCount { get; set; }
}
```

**Benefits**:
1. **Separate tracking**: Can query recent items without loading User
2. **Usage analytics**: Track which items are popular
3. **Efficient updates**: Don't load entire User object for this
4. **Scalability**: Recent items table can be archived separately

**Composite Primary Key** `(UserId, StickerId)`:
- Can't have duplicate entries
- Only one record per user-sticker pair
- Queries are fast with this key structure

---

## Conversation Entities

### Conversation
**Purpose**: Container for messages (1:1 or group).

**Why IsGroup Flag?**
```csharp
public bool IsGroup { get; set; }
```

**Rationale**:
- Different rules for 1:1 vs groups
- 1:1 can't have multiple admins
- Groups can have members, bans
- Simpler than separate entities

**URL Sharing**:
```csharp
public string? ActiveUrlSlug { get; set; }     // Shareable link
public ICollection<ConversationUrl> UrlHistory { get; set; }  // Track changes
```

**Why Track History?**
- Audit trail: who changed the URL and when
- Can revoke specific URLs
- Analytics: which links generated traffic

**Admin-Only Messaging**:
```csharp
public bool OnlyAdminsCanSendMessage { get; set; }
```

**Use Case**: Large communities can mute non-admins

### ConversationUserBan
**Purpose**: Prevent users from accessing conversations.

**Complex Design**:
```csharp
public class ConversationUserBan
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public Guid BannedByUserId { get; set; }
    
    public DateTime BannedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }        // Temporary ban
    
    public bool IsRevoked { get; set; }            // Soft revoke
    public Guid? RevokedByUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    
    public string? Reason { get; set; }            // Why banned
}
```

**Composite Key** `(ConversationId, UserId)`:
- Can't ban same user twice
- Query bans efficiently

**Why Track Who Banned?**
- Accountability: admins know who made the decision
- Can revert if wrong moderator banned someone
- Audit trail

**Why Soft Revoke Instead of Delete?**
- Historical record: can see ban was issued then revoked
- Analytics: track ban/unban patterns
- Query filter hides revoked from normal queries

**Query Filter**:
```csharp
entity.HasQueryFilter(b => !b.IsRevoked);  // Exclude revoked by default
```

**Benefit**: Queries never accidentally show revoked bans

### ConversationUserAdmin
**Purpose**: Track admin permissions per conversation.

**Why Separate Entity?**
```csharp
// Instead of:
public ConversationUser {
    public bool IsAdmin { get; set; }       // ❌ Simple but loses metadata
}

// We have:
public class ConversationUserAdmin {
    public Guid GrantedByUserId { get; set; }      // Who made them admin
    public DateTime GrantedAt { get; set; }         // When
    public string? Reason { get; set; }             // Why
}
```

**Benefits**:
1. Not every ConversationUser needs admin data
2. Optional 1:1 relationship (only if admin)
3. Track admin assignment history
4. Revoke admin without removing from conversation

---

## Media Entities

### Media
**Purpose**: Universal media storage for all file types.

**Why One Entity for Everything?**
```csharp
public MediaType Type { get; set; }  // Image, Video, Gif, Audio, VoiceMessage, etc.
```

**Instead of**:
```csharp
public Image { /* ... */ }
public Video { /* ... */ }
public Audio { /* ... */ }
public Gif { /* ... */ }  // ❌ Duplicate schemas
```

**Benefits**:
1. **DRY Principle**: Single schema for storage metadata
2. **Polymorphic**: Can attach any media type to messages/posts
3. **Efficient**: Single table scan for all media queries
4. **Flexible**: Easy to add new types

**Storage Abstraction**:
```csharp
public string Url { get; set; }                    // CDN or local path
public string? StorageProvider { get; set; }      // "AWS_S3" or "LOCAL"
public string? StorageKey { get; set; }           // Provider-specific identifier
```

**Why Abstract Storage?**
- Can move from local to S3 later
- Multi-cloud strategy support
- Easy to migrate files
- Each provider has different key format

**Content Hash**:
```csharp
public string? ContentHash { get; set; }  // SHA-256 of file
```

**Use Cases**:
- Detect duplicate uploads
- Verify file integrity
- Implement deduplication

**Moderation**:
```csharp
public ICollection<MediaModeration> ModerationFlags { get; set; }
public ICollection<SensitiveContentFlag> SensitiveContentFlags { get; set; }
```

**Why Separate Collections?**
- Multiple types of flags possible
- Different workflows for each type
- Can add/remove flags independently

### GifMetadata
**Purpose**: Searchability and discovery layer for GIFs.

**Design**:
```csharp
public class GifMetadata
{
    public Guid MediaId { get; set; }      // Points to actual GIF file
    public string? Title { get; set; }
    public List<string> Tags { get; set; }
    public string? EmojiHint { get; set; }
    public bool IsTrending { get; set; }
    public int UsageCount { get; set; }
}
```

**Why Not Store This in Media?**
- Not all media is searchable (voice messages, video recordings)
- Metadata can be null for non-GIF media
- Can update metadata without touching file
- Keeps separation: storage vs discovery

**Why Emoji Hint?**
- Quick visual categorization
- Improves search UX
- Users can find "angry face" GIF quickly
- Cultural context without language barriers

### MessageAttachment
**Purpose**: Links media to messages.

**Why Not Store Directly?**
```csharp
// Instead of:
public Message {
    public Guid? ImageMediaId { get; set; }
    public Guid? VideoMediaId { get; set; }
    public Guid? FileMediaId { get; set; }
}

// We use:
public class MessageAttachment {
    public Guid MessageId { get; set; }
    public Guid MediaId { get; set; }
    public int Order { get; set; }  // Preserve order
}
```

**Benefits**:
1. One message can have multiple media files
2. Order is preserved
3. Easy to query all media for a message
4. Reuse for PostAttachment, StoryAttachment

---

## Sticker System

### StickerPack
**Purpose**: Organize stickers into themed collections.

```csharp
public class StickerPack
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? CoverMediaId { get; set; }     // Thumbnail
    public bool IsOfficial { get; set; }        // From app or user
    public bool IsActive { get; set; }          // Soft delete
    public ICollection<Sticker> Stickers { get; set; }
}
```

**Why CoverMedia?**
- Shows preview without loading all stickers
- Faster pack browsing
- Better UX

**Why IsOfficial Flag?**
- Official packs from app vs user-created
- Different moderation policies
- Can feature official packs

### Sticker
**Purpose**: Individual sticker within a pack.

```csharp
public class Sticker
{
    public Guid StickerPackId { get; set; }
    public Guid MediaId { get; set; }          // References file
    public string? Emoji { get; set; }         // For quick search
    public int Order { get; set; }             // Position in pack
    public bool IsAnimated { get; set; }
    public bool IsActive { get; set; }         // Moderation
}
```

**Why Emoji?**
- Users remember "angry face" not sticker ID
- Faster search
- Cultural familiarity

**Why Order?**
- Preserves pack organization
- Stickers might be reordered over time
- Important for UX

---

## Summary: Design Principles

| Principle | Benefit | Example |
|-----------|---------|---------|
| **Separate Entities for Metadata** | Keeps tables clean, enables focused queries | UserRecentSticker (don't bloat User) |
| **Composite Keys** | Prevents duplicates, improves indexing | (UserId, StickerId) in UserRecentSticker |
| **Soft Deletes** | Maintains history, enables recovery | IsDeleted + DeletedAt |
| **Query Filters** | Automatic exclusion of deleted/revoked data | Global filter on ConversationUserBan |
| **Abstraction Layers** | Flexibility for future changes | Media entity abstracts storage provider |
| **Audit Trails** | Accountability and compliance | Track who banned, who granted admin |
| **Polymorphism** | Reduce duplication | One Media entity for all file types |

