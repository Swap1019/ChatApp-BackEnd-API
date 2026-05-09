# Channel Policies

Status: policy specification planned.

The current backend has channel entities and admin/ban models. Runtime policy
enforcement still needs channel policy classes, API endpoints, application
services, and authorization wiring.

Related domain entities:

- `Channel`
- `ChannelUser`
- `ChannelUserAdmin`
- `ChannelUserBan`
- `ChannelUrl`
- `ChannelPinnedMessage`

Related flags and fields:

- `Channel.CreatorUserId`
- `Channel.IsPrivate`
- `Channel.IsDiscoverable`
- `Channel.AccessCode`
- `Channel.AccessCodeExpiresAt`
- `Channel.OnlyAdminsCanSendMessage`
- `Channel.IsArchived`
- `Channel.IsDeleted`
- `ChannelUserAdmin.CanKickMembers`
- `ChannelUserAdmin.CanPinMessages`
- `ChannelUserAdmin.CanDeleteMessages`
- `ChannelUserAdmin.CanUpdateChannel`
- `ChannelUserAdmin.CanManageRoles`
- `ChannelUserAdmin.CanManageCalls`
- `ChannelUserAdmin.CanAddAdmins`
- `ChannelUserAdmin.SignName`

---

## Policy Model

Channel authorization should follow the same application-layer shape as the
existing conversation and message policies:

1. A request object identifies the actor, target channel, and target user or
   message when applicable.
2. An authorization query loads the minimum required snapshots from the database.
3. A rules class evaluates business rules and returns a failure reason.
4. The policy returns `PolicyResult.Allow()` or `PolicyResult.Deny(reason)`.

Suggested namespace layout:

```text
ChatApp.Application.Authorization.Policies.Channels.{Action}
ChatApp.Application.Authorization.Queries.Channels.{Action}
ChatApp.Application.Authorization.Rules.Channels
```

Policy checks should run before write operations and before returning private
channel data.

---

## Channel Roles

### Creator

The creator is the highest channel-level authority.

Creator can:

- Update channel metadata and settings.
- Delete or archive the channel.
- Add, remove, and update channel admins.
- Grant any channel admin permission.
- Remove subscribers.
- Ban subscribers.
- Pin and unpin channel messages.
- Delete channel messages.
- Rotate or revoke private access codes.

Creator cannot:

- Bypass platform-wide user suspension or ban rules.
- Read data hidden by a user privacy setting unless a separate privacy policy
  allows it.
- Perform platform moderation actions outside the channel.

### Channel Admin

A channel admin is represented by `ChannelUserAdmin`, attached to a
`ChannelUser` membership row. Admin authority is permission-based, not a single
boolean.

Admin can perform only actions covered by granted flags:

- `CanKickMembers`: remove subscribers.
- `CanPinMessages`: pin and unpin channel messages.
- `CanDeleteMessages`: delete channel messages.
- `CanUpdateChannel`: edit channel name, description, avatar, discoverability,
  privacy, and send-mode settings.
- `CanManageRoles`: update existing admin permissions.
- `CanManageCalls`: manage channel calls or live sessions when that feature
  exists.
- `CanAddAdmins`: promote subscribers to admin.
- `SignName`: show the admin identity when posting as the channel.

Admin cannot:

- Delete the channel unless they are also the creator.
- Grant permissions they do not hold, unless the actor is the creator.
- Modify the creator's admin status.
- Remove or ban the creator.

### Subscriber

A subscriber is represented by `ChannelUser`.

Subscriber can:

- View channel content when the channel is not deleted and the user is not
  banned.
- Send messages when the channel is not archived and
  `OnlyAdminsCanSendMessage` is false.
- React to visible messages.
- Leave the channel.

Subscriber cannot:

- Update channel settings.
- Manage subscribers or admins.
- Pin or delete other users' messages.
- Send messages while banned or when only admins can send.

### Public Viewer

A public viewer is an authenticated user who is not subscribed to a public
channel.

Public viewer can:

- View public, discoverable channel preview data.
- View public channel history if the product intentionally supports read-only
  public channel viewing.
- Join the channel if not banned.

Public viewer cannot:

- View private channels.
- Send messages.
- React to messages.
- See subscriber-only metadata.

---

## Policy Precedence

Channel policy decisions should use this precedence:

```text
Platform deny
  > channel deleted
  > channel archived write restrictions
  > active channel ban
  > creator
  > explicit admin permission
  > subscriber rights
  > public viewer rights
  > default deny
```

Platform-level deny includes global user bans, user suspensions, invalid
sessions, and missing authentication.

Platform admin override should be handled separately from normal channel
policies and must create an audit log entry.

---

## Channel Action Policies

### Create Channel

Allow when:

- Actor is authenticated.
- Actor is not globally banned or suspended.
- Actor has the platform permission to create channels, if platform permissions
  are enforced for this action.
- Name and required metadata pass validation.

Deny when:

- Actor is blocked by platform safety checks.
- Requested channel URL or slug is unavailable.
- Private channel is requested without a valid access-code strategy.

Suggested policy:

- `CreateChannelPolicy`
- `CreateChannelAuthorizationQuery`
- `CreateChannelRules`

### View Channel

Allow when:

- Actor is the creator.
- Actor is a subscriber and is not banned.
- Actor is an admin through a subscriber row and is not banned.
- Channel is public, not deleted, and public preview or public read access is
  allowed.

Deny when:

- Channel is deleted.
- Actor is banned from the channel.
- Channel is private and actor is not subscribed.
- Channel is not discoverable and actor only has public-viewer access.

Private channel responses should avoid leaking whether the channel exists unless
the actor has an invitation, access code, or membership.

Suggested policy:

- `ViewChannelPolicy`
- `ViewChannelAuthorizationQuery`
- `ViewChannelRules`

### Join Channel

Allow when:

- Actor is authenticated.
- Actor is not globally banned or suspended.
- Channel exists and is not deleted.
- Channel is not archived, unless archived channels intentionally allow joining.
- Actor is not already subscribed.
- Actor is not banned from the channel.
- Public channel allows open joins.
- Private channel access code is present, matches, and is not expired.

Deny when:

- Access code is invalid or expired.
- Actor is banned from the channel.
- Channel is private and actor has no valid invite/access code.
- Actor is already subscribed.

Suggested policy:

- `JoinChannelPolicy`
- `JoinChannelAuthorizationQuery`
- `JoinChannelRules`

### Leave Channel

Allow when:

- Actor is subscribed.
- Actor is not the creator, or ownership is transferred/deletion is completed
  first.

Deny when:

- Actor is not subscribed.
- Actor is the creator and the channel would be left without an owner.

Suggested policy:

- `LeaveChannelPolicy`
- `LeaveChannelAuthorizationQuery`
- `LeaveChannelRules`

### Update Channel

Allow when:

- Actor is creator.
- Actor is an admin with `CanUpdateChannel`.

Deny when:

- Channel is deleted.
- Actor is banned.
- Actor attempts to update protected fields without creator authority.
- Actor attempts to rotate access codes without update authority.

Settings covered by this policy:

- Name.
- Description.
- Avatar.
- Privacy mode.
- Discoverability.
- `OnlyAdminsCanSendMessage`.
- Access code rotation.

Suggested policy:

- `UpdateChannelPolicy`
- `UpdateChannelAuthorizationQuery`
- `UpdateChannelRules`

### Archive Channel

Allow when:

- Actor is creator.
- Actor has a platform permission such as `ArchiveChannel`.

Deny when:

- Channel is already deleted.
- Actor is only an admin.

Archive behavior:

- Set `IsArchived`.
- Set `ArchivedAt`.
- Set `ArchivedBy`.
- Stop normal message sends.
- Keep history visible to actors who can view the channel.

Suggested policy:

- `ArchiveChannelPolicy`
- `ArchiveChannelAuthorizationQuery`
- `ArchiveChannelRules`

### Delete Channel

Allow when:

- Actor is creator.
- Actor has a platform moderation/admin override permission.

Deny when:

- Actor is only a channel admin.
- Channel is already deleted.

Delete behavior:

- Soft-delete with `IsDeleted`, `DeletedAt`, and `DeletedBy`.
- Hide channel from search and normal listing.
- Block joins, message sends, reactions, pins, and admin changes.
- Keep data available for audit/moderation flows.

Suggested policy:

- `DeleteChannelPolicy`
- `DeleteChannelAuthorizationQuery`
- `DeleteChannelRules`

### Add Subscriber

Allow when:

- Actor is creator.
- Actor is admin with `CanKickMembers` or a future explicit add-member flag.
- Target user can receive channel invitations under the privacy layer.
- Target user is not globally banned or suspended.
- Target user is not banned from the channel.
- Target user is not already subscribed.

Deny when:

- Target user's privacy settings reject invitations.
- Target user has blocked the actor.
- Actor lacks member-management authority.

Because `ChannelUserAdmin` currently has `CanKickMembers` but no
`CanAddMembers`, consider adding a dedicated permission if channels support
admin-driven subscriber invites.

Suggested policy:

- `AddChannelSubscriberPolicy`
- `AddChannelSubscriberAuthorizationQuery`
- `AddChannelSubscriberRules`

### Remove Subscriber

Allow when:

- Actor is creator.
- Actor is admin with `CanKickMembers`.
- Actor removes themselves through the leave-channel flow.

Deny when:

- Target is the creator.
- Target is not subscribed.
- Actor tries to remove an admin with equal or higher authority and is not the
  creator.

Suggested policy:

- `RemoveChannelSubscriberPolicy`
- `RemoveChannelSubscriberAuthorizationQuery`
- `RemoveChannelSubscriberRules`

### Ban User

Allow when:

- Actor is creator.
- Actor is admin with `CanKickMembers`.
- Target user is not the creator.
- Target user is not a higher-authority admin.

Deny when:

- Target is already banned.
- Actor attempts to ban themselves.
- Actor lacks member-management authority.

Ban behavior:

- Create `ChannelUserBan`.
- Remove or disable the subscriber row if present.
- Prevent joining, viewing private content, sending messages, reacting, and
  receiving channel events.

Suggested policy:

- `BanChannelUserPolicy`
- `BanChannelUserAuthorizationQuery`
- `BanChannelUserRules`

### Unban User

Allow when:

- Actor is creator.
- Actor is admin with `CanKickMembers`.

Deny when:

- Ban record does not exist.
- Actor lacks member-management authority.

Suggested policy:

- `UnbanChannelUserPolicy`
- `UnbanChannelUserAuthorizationQuery`
- `UnbanChannelUserRules`

### Add Admin

Allow when:

- Actor is creator.
- Actor is admin with `CanAddAdmins`.
- Target is subscribed.
- Target is not banned.
- Requested permissions are a subset of actor's permissions, unless actor is
  creator.

Deny when:

- Target is already admin.
- Actor tries to grant permissions they do not hold.
- Actor lacks admin-management authority.

Suggested policy:

- `AddChannelAdminPolicy`
- `AddChannelAdminAuthorizationQuery`
- `AddChannelAdminRules`

### Update Admin Permissions

Allow when:

- Actor is creator.
- Actor is admin with `CanManageRoles`.
- Target admin exists.
- Requested permissions do not exceed actor authority, unless actor is creator.

Deny when:

- Target is creator.
- Actor tries to modify their own role in a way that escalates authority.
- Actor tries to grant `CanManageRoles` or `CanAddAdmins` without already
  holding those permissions.

Suggested policy:

- `UpdateChannelAdminPermissionsPolicy`
- `UpdateChannelAdminPermissionsAuthorizationQuery`
- `UpdateChannelAdminPermissionsRules`

### Remove Admin

Allow when:

- Actor is creator.
- Actor is admin with `CanManageRoles`.
- Target admin exists.

Deny when:

- Target is creator.
- Actor attempts to remove their own last management permission.
- Actor has lower authority than the target admin.

Suggested policy:

- `RemoveChannelAdminPolicy`
- `RemoveChannelAdminAuthorizationQuery`
- `RemoveChannelAdminRules`

### Send Channel Message

Allow when:

- Actor is authenticated.
- Actor is subscribed.
- Actor is not banned from the channel.
- Channel is not deleted.
- Channel is not archived.
- `OnlyAdminsCanSendMessage` is false, or actor is creator/admin.

Deny when:

- Actor only has public-view access.
- Actor is banned.
- Channel is archived or deleted.
- Channel only allows admin sends and actor has no channel admin authority.

Suggested policy:

- `SendChannelMessagePolicy`
- `SendChannelMessageAuthorizationQuery`
- `SendChannelMessageRules`

### Pin Channel Message

Allow when:

- Actor is creator.
- Actor is admin with `CanPinMessages`.
- Message belongs to the channel.
- Message is not deleted.

Deny when:

- Channel is deleted.
- Message is not in the target channel.
- Actor lacks pin authority.

Suggested policy:

- `PinChannelMessagePolicy`
- `PinChannelMessageAuthorizationQuery`
- `PinChannelMessageRules`

### Delete Channel Message

Allow when:

- Actor is the message sender and the product allows sender deletion.
- Actor is creator.
- Actor is admin with `CanDeleteMessages`.
- Actor has platform moderation authority.

Deny when:

- Message is already deleted.
- Message does not belong to the channel.
- Actor lacks sender, channel admin, or platform moderation authority.

Suggested policy:

- `DeleteChannelMessagePolicy`
- `DeleteChannelMessageAuthorizationQuery`
- `DeleteChannelMessageRules`

---

## Visibility Rules

Channel responses should be shaped by access level:

- Creator/admin: full channel metadata, subscribers if supported, admin
  permissions, bans if authorized, pinned messages, and settings.
- Subscriber: channel metadata, visible messages, pinned messages, own
  membership state, and safe subscriber-visible counts.
- Public viewer: preview metadata only, plus public history if the product
  chooses to support it.
- Denied actor: no private metadata; use `404`-style responses for private or
  non-discoverable channels when appropriate.

Never expose:

- Private access code hash/raw value in normal reads.
- Ban list to non-admins.
- Admin permission details to normal subscribers unless intentionally exposed.
- User fields blocked by the privacy layer.

---

## Query Data Required

Most channel policies need a small shared snapshot:

```csharp
public sealed class ChannelAuthorizationData
{
    public ChannelSnapshot? Channel { get; init; }
    public UserSnapshot? Actor { get; init; }
    public ChannelMemberSnapshot? ActorMembership { get; init; }
    public ChannelAdminSnapshot? ActorAdmin { get; init; }
    public bool IsActorBanned { get; init; }
    public bool IsActorCreator { get; init; }
}
```

Targeted actions may also need:

- Target user snapshot.
- Target membership snapshot.
- Target admin snapshot.
- Target ban snapshot.
- Message snapshot.
- Access-code validation result.
- Privacy-layer decision for invitations or user data disclosure.

---

## Implementation Checklist

- [ ] Add channel policy request types.
- [ ] Add channel authorization query types.
- [ ] Add channel rules classes.
- [ ] Register channel policies in application dependency injection.
- [ ] Add channel service methods that call policies before mutation.
- [ ] Add channel API endpoints.
- [ ] Add tests for creator, admin permission, subscriber, public viewer,
      private channel, ban, archive, and delete paths.
- [ ] Add audit logging for admin changes, bans, archive/delete, and platform
      overrides.
