# Privacy Policies

Status: privacy-layer specification planned.

The current backend has privacy entities and enums. Runtime privacy enforcement
still needs services, endpoints, and tests.

Related domain entities and enums:

- `UserPrivacy`
- `UserPrivacyException`
- `PrivacyLevel`
- `PrivacyField`
- `Contact`
- `BlockedUser`

The privacy layer answers one central question:

```text
Can actor user A view or use field/resource R owned by target user B?
```

It should be used by API reads and by action policies that depend on another
user's preferences, such as invitations, direct messages, calls, forwarded
message attribution, stories, posts, profile views, and search.

---

## Core Principle

Authorization decides whether the actor can perform an action on a resource.
Privacy decides how much user-owned data the actor is allowed to see or use.

Examples:

- Message policy decides whether a user can send a message.
- Privacy policy decides whether the target allows direct messages from that
  actor.
- Channel policy decides whether an actor can add a subscriber.
- Privacy policy decides whether the target allows channel/group invitations.
- Profile endpoint decides whether the actor can request a profile.
- Privacy policy decides which fields are visible in the response.

These systems should be composed, not merged into one large rule class.

---

## Privacy Levels

`PrivacyLevel` currently supports:

- `Everyone`: Any authenticated viewer can see or use the field.
- `ContactsOnly`: Only contacts of the target user can see or use the field.
- `Nobody`: No other user can see or use the field.
- `Custom`: Use `UserPrivacyException` records to decide actor-specific access.

The target user can always see their own data unless the account is deleted or
the data itself no longer exists.

Platform admin override should not be part of normal privacy evaluation. It
should be a separate audited path with a reason code.

---

## Privacy Fields

`PrivacyField` currently includes:

- `PhoneNumber`
- `SearchByPhoneNumberPrivacy`
- `Email`
- `LastSeenAndOnline`
- `ProfilePhoto`
- `ForwardedMessages`
- `Call`
- `Bio`
- `GroupInvitations`
- `ReadReceipts`
- `AllowDirectMessages`
- `Stories`
- `Posts`

The matching `UserPrivacy` fields are:

- `PhoneNumberPrivacy`
- `SearchByPhoneNumberPrivacy`
- `EmailPrivacy`
- `LastSeenAndOnlinePrivacy`
- `ProfilePhotoPrivacy`
- `ForwardedMessagesPrivacy`
- `CallPrivacy`
- `BioPrivacy`
- `GroupInvitationsPrivacy`
- `ReadReceiptsPrivacy`
- `AllowDirectMessagesPrivacy`
- `StoriesPrivacy`
- `PostsPrivacy`

The privacy layer should own the mapping between `PrivacyField` and the
corresponding `UserPrivacy` property so endpoint and policy code does not repeat
switch statements.

---

## Decision Precedence

Privacy decisions should use this order:

```text
missing actor authentication
  > missing target user
  > target self-access
  > block relationship
  > explicit deny exception
  > explicit allow exception
  > field privacy level
  > default deny
```

Rules:

- If the actor is not authenticated, deny private user data by default.
- If actor and target are the same user, allow.
- If either user has blocked the other, deny interpersonal fields and actions.
- A `UserPrivacyException` with `IsAllowed = false` denies access.
- A `UserPrivacyException` with `IsAllowed = true` allows access.
- If no exception applies, evaluate the target user's configured
  `PrivacyLevel`.

For `Custom`, deny when no allow exception exists.

---

## Field Rules

### Phone Number

Used by:

- Profile views.
- Contact discovery.
- Account lookup flows that intentionally expose a phone match.

Allow when:

- Actor is target.
- Target privacy is `Everyone`.
- Target privacy is `ContactsOnly` and actor is in target contacts.
- Target has an allow exception for actor.

Deny when:

- Target privacy is `Nobody`.
- Target has a deny exception for actor.
- Actor and target are blocked in either direction.

### Search By Phone Number

Used by:

- Search and contact-discovery flows.

Allow when:

- The search input matches the target user's phone number.
- `SearchByPhoneNumberPrivacy` allows the actor.

Deny behavior:

- Return no match.
- Do not reveal that a hidden user exists.

### Email

Used by:

- Profile views.
- Account lookup flows if email discovery is intentionally supported.

Default response shaping should hide email from other users unless explicitly
allowed.

### Last Seen And Online

Used by:

- Presence indicators.
- Conversation headers.
- Contact lists.

Allow when the field privacy allows actor access.

Deny behavior:

- Omit exact timestamps.
- Return a coarse status such as unavailable if the API requires a stable shape.

### Profile Photo

Used by:

- Profile cards.
- Conversations.
- Channel subscriber lists.
- Search results.

Deny behavior:

- Return no avatar or a default avatar.
- Do not expose the media URL if privacy denies access.

### Forwarded Messages

Used by:

- Forward attribution.
- "Forwarded from" labels.

Allow when the target allows attribution for the actor.

Deny behavior:

- Show generic forwarded text without linking to the target user.

### Call

Used by:

- Audio/video call initiation.
- Future channel or group call invitations when a user is directly called.

Allow when:

- Actor is target.
- Actor is permitted by `CallPrivacy`.
- Actor is not blocked by target.

Deny behavior:

- Reject the call request with a privacy-safe failure reason.

### Bio

Used by:

- Full profile views.
- Search profile preview if supported.

Deny behavior:

- Omit bio from the response.

### Group Invitations

Used by:

- Conversation add-user policy.
- Channel add-subscriber or invite policy.

Allow when:

- Target's `GroupInvitationsPrivacy` allows the actor.
- Actor is not blocked by target.

Deny behavior:

- Policy should return a failure reason such as `Target user does not allow
  invites`.

### Read Receipts

Used by:

- Message read receipt display.

Allow when:

- Target user allows read receipts for the actor.
- The message/conversation policy also allows the actor to view the message
  context.

Deny behavior:

- Do not expose per-user read rows for that target.
- Aggregate counts must avoid making hidden reads inferable.

### Allow Direct Messages

Used by:

- Direct conversation creation.
- Direct message send policy.

Allow when:

- Target's `AllowDirectMessagesPrivacy` allows the actor.
- Neither user has blocked the other.

Deny behavior:

- Prevent direct conversation creation or message send.

### Stories

Used by:

- Story list.
- Story details.
- Story seen events.

Allow when:

- Target's `StoriesPrivacy` allows the actor.
- Story-specific visibility rules also allow the actor, if those are added.

Deny behavior:

- Do not include the story in lists.
- Do not allow creating a `StorySeen` row.

### Posts

Used by:

- Profile post lists.
- Feed eligibility.
- Post details.

Allow when:

- Target's `PostsPrivacy` allows the actor.
- Post-specific visibility rules also allow the actor, if those are added.

Deny behavior:

- Do not include the post in lists.
- Return a privacy-safe not-found response for direct requests.

---

## Exception Rules

`UserPrivacyException` is scoped by:

- `OwnerUserId`: the user whose privacy is being controlled.
- `TargetUserId`: the actor affected by the exception.
- `Field`: the privacy field.
- `IsAllowed`: allow or deny.

Rules:

- There should be at most one exception per `(OwnerUserId, TargetUserId, Field)`.
- Deny exceptions should win over allow exceptions if duplicates ever exist.
- Exceptions apply before the general privacy level.
- Exceptions should not override block relationships unless the product
  explicitly supports that behavior.
- Exceptions should be ignored for target self-access.

Recommended database constraint:

```text
Unique(OwnerUserId, TargetUserId, Field)
```

---

## Privacy Service Contract

Add an application-layer service that all endpoints and policies can call:

```csharp
public interface IUserPrivacyService
{
    Task<PrivacyDecision> CanViewAsync(
        Guid actorUserId,
        Guid targetUserId,
        PrivacyField field,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<PrivacyField, PrivacyDecision>> CanViewManyAsync(
        Guid actorUserId,
        Guid targetUserId,
        IReadOnlyCollection<PrivacyField> fields,
        CancellationToken cancellationToken = default);
}
```

Suggested result type:

```csharp
public sealed class PrivacyDecision
{
    public bool IsAllowed { get; init; }
    public string? Reason { get; init; }
    public PrivacyField Field { get; init; }
    public PrivacyLevel? EffectiveLevel { get; init; }
    public bool UsedException { get; init; }
}
```

Reason values should be stable enough for logging and tests, but API responses
should avoid exposing sensitive relationship details.

---

## Privacy Data Query

The privacy service should load only the data needed for the requested fields:

```csharp
public sealed class PrivacyAuthorizationData
{
    public UserPrivacy? TargetPrivacy { get; init; }
    public bool ActorExists { get; init; }
    public bool TargetExists { get; init; }
    public bool IsSelf { get; init; }
    public bool IsActorInTargetContacts { get; init; }
    public bool IsBlockedByActor { get; init; }
    public bool IsBlockedByTarget { get; init; }
    public IReadOnlyCollection<UserPrivacyException> Exceptions { get; init; }
        = Array.Empty<UserPrivacyException>();
}
```

Batch reads should evaluate multiple fields from one loaded snapshot to avoid
N+1 lookups when shaping profile responses.

---

## Response Shaping

Privacy checks should happen before serializing user-owned fields.

Example profile shaping:

```csharp
var decisions = await privacyService.CanViewManyAsync(
    actorUserId,
    targetUserId,
    new[]
    {
        PrivacyField.ProfilePhoto,
        PrivacyField.Bio,
        PrivacyField.LastSeenAndOnline,
        PrivacyField.PhoneNumber,
        PrivacyField.Email,
        PrivacyField.Posts
    },
    cancellationToken);

return new UserProfileDto
{
    Id = target.Id,
    Username = target.Username,
    DisplayName = target.DisplayName,
    AvatarUrl = decisions[PrivacyField.ProfilePhoto].IsAllowed
        ? target.AvatarUrl
        : null,
    Bio = decisions[PrivacyField.Bio].IsAllowed
        ? target.Bio
        : null,
    LastSeenAt = decisions[PrivacyField.LastSeenAndOnline].IsAllowed
        ? target.LastSeenAt
        : null,
    PhoneNumber = decisions[PrivacyField.PhoneNumber].IsAllowed
        ? target.PhoneNumber
        : null,
    Email = decisions[PrivacyField.Email].IsAllowed
        ? target.Email
        : null
};
```

Endpoint code should not call privacy rules field-by-field if a batch decision
can be made once.

---

## Integration With Action Policies

Privacy should be injected into authorization queries or services when an
action depends on target-user preferences.

Examples:

- `AddUserPolicy` for conversations already needs group-invitation privacy.
- Channel invite/add-subscriber policy should check `GroupInvitations`.
- Direct message policy should check `AllowDirectMessages`.
- Call policy should check `Call`.
- Story seen policy should check `Stories`.
- Read receipt display should check `ReadReceipts`.

For consistency, action policies should receive normalized booleans from their
authorization query, such as `TargetUserAllowsInvites`, instead of directly
inspecting `UserPrivacy` in the rules class.

---

## API Behavior

Privacy-denied reads should usually be shaped as missing fields, not hard
failures.

Use shaped responses when:

- Viewing a profile where some fields are hidden.
- Listing contacts or channel subscribers.
- Rendering message headers, avatars, or presence.

Use denial responses when:

- Actor requests a resource that should not be visible at all.
- Actor tries to start a call, direct message, invitation, or story view that
  privacy rejects.

For private discovery flows, prefer not-found style responses over explicit
privacy errors to avoid leaking hidden users.

---

## Defaults

Current `UserPrivacy` defaults are `ContactsOnly` for all fields. New user
creation should create a `UserPrivacy` row with those defaults.

If a `UserPrivacy` row is missing:

- Create one lazily if the write path can do so safely.
- Otherwise evaluate as `ContactsOnly` for compatibility with current domain
  defaults.
- Log the missing row as data repair signal.

---

## Auditing And Logging

Log privacy setting changes:

- Field changed.
- Previous level.
- New level.
- Actor user id.
- Owner user id.
- Timestamp.

Log privacy exception changes:

- Owner user id.
- Target user id.
- Field.
- `IsAllowed`.
- Created/updated/deleted timestamp.

Do not log sensitive field values such as phone numbers, emails, or raw profile
content as part of privacy decision logs.

Admin privacy overrides must be audited separately with:

- Admin user id.
- Target user id.
- Fields accessed.
- Reason code.
- Ticket/case id if available.
- Timestamp.

---

## Implementation Checklist

- [ ] Add `IUserPrivacyService`.
- [ ] Add privacy decision and query data types.
- [ ] Add field-to-level mapping helper.
- [ ] Add batch privacy evaluation.
- [ ] Add uniqueness constraint for privacy exceptions.
- [ ] Wire privacy checks into profile/search/read endpoints when those exist.
- [ ] Wire privacy checks into conversation, channel, call, story, and direct
      message policies.
- [ ] Add tests for every `PrivacyLevel`.
- [ ] Add tests for allow and deny exceptions.
- [ ] Add tests for block precedence.
- [ ] Add tests for self-access.
- [ ] Add tests for response shaping.
