using ChatApp.Domain.Entities.Channel;
using ChatApp.Domain.Entities.Conversation;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Domain.Entities.Media;
using ChatApp.Domain.Entities.Messaging;
using ChatApp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserToken> UserTokens { get; }
    DbSet<UserSession> UserSessions { get; }
    DbSet<UserSuspension> UserSuspensions { get; }
    DbSet<UserPrivacy> UserPrivacies { get; }
    DbSet<UserPrivacyException> UserPrivacyExceptions { get; }

    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserRole> UserRoles { get; }

    DbSet<Conversation> Conversations { get; }
    DbSet<ConversationUser> ConversationUsers { get; }
    DbSet<ConversationUserAdmin> ConversationUserAdmins { get; }
    DbSet<ConversationUserBan> ConversationUserBans { get; }
    DbSet<ConversationUrl> ConversationUrls { get; }

    DbSet<Message> Messages { get; }
    DbSet<MessageThread> MessageThreads { get; }
    DbSet<MessageRead> MessageReads { get; }
    DbSet<MessageReaction> MessageReactions { get; }
    DbSet<MessageDeletion> MessageDeletions { get; }
    DbSet<PinnedMessage> PinnedMessages { get; }

    DbSet<Media> Media { get; }
    DbSet<Avatar> Avatars { get; }
    DbSet<MediaModeration> MediaModerations { get; }
    DbSet<MessageAttachment> MessageAttachments { get; }
    DbSet<PostAttachment> PostAttachments { get; }
    DbSet<SensitiveContentFlag> SensitiveContentFlags { get; }
    DbSet<GifMetadata> GifMetadatas { get; }
    DbSet<UserRecentGif> UserRecentGifs { get; }
    DbSet<StickerPack> StickerPacks { get; }
    DbSet<Sticker> Stickers { get; }
    DbSet<UserStickerPack> UserStickerPacks { get; }
    DbSet<UserRecentSticker> UserRecentStickers { get; }

    DbSet<Channel> Channels { get; }
    DbSet<ChannelUser> ChannelUsers { get; }
    DbSet<ChannelUserAdmin> ChannelUserAdmins { get; }
    DbSet<ChannelUserBan> ChannelUserBans { get; }
    DbSet<ChannelUrl> ChannelUrls { get; }
    DbSet<ChannelPinnedMessage> ChannelPinnedMessages { get; }

    DbSet<Contact> Contacts { get; }
    DbSet<BlockedUser> BlockedUsers { get; }
    DbSet<Post> Posts { get; }
    DbSet<PostLike> PostLikes { get; }
    DbSet<Story> Stories { get; }
    DbSet<StorySeen> StorySeens { get; }

    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
