using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Domain.Entities.Identity;
using ChatApp.Domain.Entities.Conversation;
using ChatApp.Domain.Entities.Media;
using ChatApp.Domain.Entities.Messaging;
using ChatApp.Domain.Entities.Social;
using ChatApp.Domain.Entities.Channel;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<UserSuspension> UserSuspensions { get; set; }
        public DbSet<UserPrivacy> UserPrivacies { get; set; }
        public DbSet<UserPrivacyException> UserPrivacyExceptions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationUser> ConversationUsers { get; set; }
        public DbSet<ConversationUserBan> ConversationUserBans { get; set; }
        public DbSet<ConversationUserAdmin> ConversationUserAdmins { get; set; }
        public DbSet<ConversationUrl> ConversationUrls { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageThread> MessageThreads { get; set; }
        public DbSet<MessageRead> MessageReads { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<MessageDeletion> MessageDeletions { get; set; }
        public DbSet<PinnedMessage> PinnedMessages { get; set; }
        public DbSet<StickerPack> StickerPacks { get; set; }
        public DbSet<Sticker> Stickers { get; set; }
        public DbSet<UserStickerPack> UserStickerPacks { get; set; }
        public DbSet<UserRecentSticker> UserRecentStickers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<Story> Stories { get; set; }
        public DbSet<StorySeen> StorySeens { get; set; }

        public DbSet<Media> Media { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<MediaModeration> MediaModerations { get; set; }
        public DbSet<SensitiveContentFlag> SensitiveContentFlags { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<GifMetadata> GifMetadatas { get; set; }
        public DbSet<UserRecentGif> UserRecentGifs { get; set; }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelUser> ChannelUsers { get; set; }
        public DbSet<ChannelUserAdmin> ChannelUserAdmins { get; set; }
        public DbSet<ChannelUserBan> ChannelUserBans { get; set; }
        public DbSet<ChannelUrl> ChannelUrls { get; set; }
        public DbSet<ChannelPinnedMessage> ChannelPinnedMessages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================== USER =====================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.PhoneNumber).IsUnique();
            });

            // ===================== USER TOKEN =====================
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasOne(ut => ut.User)
                    .WithMany(u => u.Tokens)
                    .HasForeignKey(ut => ut.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== USER SESSION =====================
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasOne(us => us.User)
                    .WithMany(u => u.Sessions)
                    .HasForeignKey(us => us.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== USER SUSPENSION =====================
            modelBuilder.Entity<UserSuspension>(entity =>
            {
                entity.HasQueryFilter(us => !us.IsDeleted);

                entity.HasOne(us => us.User)
                    .WithMany(u => u.Suspensions)
                    .HasForeignKey(us => us.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(us => us.SuspendedByUser)
                    .WithMany()
                    .HasForeignKey(us => us.SuspendedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== User Privacy =====================
            modelBuilder.Entity<UserPrivacy>(entity => 
            {
                entity.HasKey(up => up.UserId);

                entity.HasOne(up => up.User)
                    .WithOne()
                    .HasForeignKey<UserPrivacy>(up => up.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== User Privacy Exception =====================
            modelBuilder.Entity<UserPrivacyException>(entity =>
            {
                entity.HasKey(upe => upe.Id);

                entity.HasOne(upe => upe.OwnerUser)
                    .WithMany(u => u.PrivacyExceptionsAsOwner)
                    .HasForeignKey(upe => upe.OwnerUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(upe => upe.TargetUser)
                    .WithMany(u => u.PrivacyExceptionsAsTarget)
                    .HasForeignKey(upe => upe.TargetUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== PERMISSION =====================
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Codename).IsUnique();
                entity.Property(p => p.Codename).HasMaxLength(100);
                entity.Property(p => p.Name).HasMaxLength(255);
            });

            // ===================== ROLE =====================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.Name).HasMaxLength(100);

            });

            // ===================== ROLE PERMISSION =====================
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                entity.HasIndex(rp => rp.PermissionId);

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.Permissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== USER ROLE =====================
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasIndex(ur => ur.RoleId);

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.Roles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CONVERSATION =====================
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasQueryFilter(m => !m.IsDeleted);

                entity.HasIndex(c => c.ActiveUrlSlug).IsUnique();

                entity.HasOne(c => c.CreatedBy)
                    .WithMany(u => u.CreatedConversations)
                    .HasForeignKey(c => c.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.UrlHistory)
                    .WithOne(cu => cu.Conversation)
                    .HasForeignKey(cu => cu.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CONVERSATION USER =====================
            modelBuilder.Entity<ConversationUser>(entity =>
            {
                entity.HasKey(cu => new { cu.ConversationId, cu.UserId });

                entity.HasIndex(cu => cu.UserId);
                entity.HasIndex(cu => cu.ConversationId);

                entity.HasOne(cu => cu.Conversation)
                    .WithMany(c => c.Members)
                    .HasForeignKey(cu => cu.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cu => cu.User)
                    .WithMany(u => u.Conversations)
                    .HasForeignKey(cu => cu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CONVERSATION ADMIN =====================
            modelBuilder.Entity<ConversationUserAdmin>(entity =>
            {
                entity.HasKey(cua => cua.ConversationUserId);

                entity.HasOne(cua => cua.ConversationUser)
                    .WithOne(cu => cu.AdminPermissions)
                    .HasForeignKey<ConversationUserAdmin>(cua => cua.ConversationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(cua => cua.GrantedByUser)
                    .WithMany()
                    .HasForeignKey(cua => cua.GrantedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint on (ConversationUserId, GrantedByUserId) to prevent duplicate grants
                entity.HasIndex(cua => new { cua.ConversationUserId, cua.GrantedByUserId }).IsUnique();
            });

            // ===================== CONVERSATION USER BAN =====================
            modelBuilder.Entity<ConversationUserBan>(entity =>
            {
                entity.HasKey(b => new { b.ConversationId, b.UserId });

                // Query filter to exclude revoked bans from default queries
                entity.HasQueryFilter(b => !b.IsRevoked);

                // Performance indexes
                entity.HasIndex(b => b.UserId);
                entity.HasIndex(b => b.BannedByUserId);
                entity.HasIndex(b => b.RevokedByUserId);
                entity.HasIndex(b => b.BannedAt);
                entity.HasIndex(b => new { b.ConversationId, b.ExpiresAt });
                entity.HasIndex(b => b.IsRevoked);

                // Unique constraint on (ConversationId, UserId, BannedByUserId) to track ban hierarchy
                entity.HasIndex(b => new { b.ConversationId, b.UserId, b.BannedByUserId }).IsUnique();

                entity.HasOne(b => b.Conversation)
                    .WithMany(c => c.BannedUsers)
                    .HasForeignKey(b => b.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.User)
                    .WithMany(u => u.ConversationBans)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.BannedByUser)
                    .WithMany(u => u.BansIssued)
                    .HasForeignKey(b => b.BannedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.RevokedByUser)
                    .WithMany()
                    .HasForeignKey(b => b.RevokedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(b => b.BannedAt).IsRequired();
                entity.Property(b => b.Reason).HasMaxLength(500);
            });

            // ===================== CONVERSATION URL =====================
            modelBuilder.Entity<ConversationUrl>(entity =>
            {
                entity.HasKey(cu => cu.Id);

                entity.HasIndex(cu => cu.ConversationId);
                entity.HasIndex(cu => cu.UrlSlug).IsUnique();
                entity.HasIndex(cu => cu.IsActive);

                entity.HasOne(cu => cu.Conversation)
                    .WithMany(c => c.UrlHistory)
                    .HasForeignKey(cu => cu.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(cu => cu.UrlSlug)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // ===================== MESSAGE =====================
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasIndex(m => new { m.ConversationId, m.Sequence });
                entity.HasIndex(m => m.SenderId);
                entity.HasQueryFilter(m => !m.IsDeleted);

                entity.HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Sender)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ForwardedFromMessage)
                    .WithMany()
                    .HasForeignKey(m => m.ForwardedFromMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Thread)
                    .WithMany(t => t.Messages)
                    .HasForeignKey(m => m.ThreadId)
                    .OnDelete(DeleteBehavior.Restrict);

                // GIF relationship - optional GIF media in a message
                entity.HasOne(m => m.GifMedia)
                    .WithMany()
                    .HasForeignKey(m => m.GifMediaId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                entity.HasIndex(m => m.CreatedAt);

                entity.HasIndex(m => new { m.ConversationId, m.CreatedAt });

                entity.HasIndex(m => m.ThreadId);

                entity.Property(m => m.Content)
                    .IsRequired()
                    .HasMaxLength(4000);
            });

            // ===================== PINNED MESSAGE =====================
            modelBuilder.Entity<PinnedMessage>(entity =>
            {

                entity.HasIndex(x => new { x.ConversationId, x.Order }).IsUnique();

                entity.HasOne(pm => pm.Message)
                    .WithMany()
                    .HasForeignKey(pm => pm.MessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pm => pm.Conversation)
                    .WithMany(c => c.PinnedMessages)
                    .HasForeignKey(pm => pm.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pm => pm.PinnedByUser)
                    .WithMany()
                    .HasForeignKey(pm => pm.PinnedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // ===================== MESSAGE DELETION =====================
            modelBuilder.Entity<MessageDeletion>(entity =>
            {
                entity.HasKey(x => new { x.MessageId, x.UserId });

                entity.HasIndex(x => x.DeletedByUserId);
                entity.HasIndex(x => x.DeletedAt);

                entity.HasOne<Message>()
                    .WithMany(m => m.Deletions)
                    .HasForeignKey(x => x.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== MESSAGE REACTIONS =====================
            modelBuilder.Entity<MessageReaction>(entity =>
            {
                entity.HasKey(x => new { x.MessageId, x.UserId, x.Emoji });

                entity.HasOne(mr => mr.Message)
                    .WithMany(m => m.Reactions)
                    .HasForeignKey(mr => mr.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mr => mr.User)
                    .WithMany()
                    .HasForeignKey(mr => mr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== MESSAGE THREAD =====================
            modelBuilder.Entity<MessageThread>(entity =>
            {
                entity.HasOne(t => t.RootMessage)
                    .WithOne()
                    .HasForeignKey<MessageThread>(t => t.RootMessageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.LastMessage)
                    .WithMany()
                    .HasForeignKey(t => t.LastMessageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== MESSAGE READ =====================
            modelBuilder.Entity<MessageRead>(entity =>
            {
                entity.HasKey(x => new { x.MessageId, x.UserId });

                entity.HasIndex(x => x.UserId);
                entity.HasIndex(x => x.ReadAt);

                entity.HasOne(mr => mr.Message)
                    .WithMany(m => m.Reads)
                    .HasForeignKey(mr => mr.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mr => mr.User)
                    .WithMany()
                    .HasForeignKey(mr => mr.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== STICKER PACK =====================
            modelBuilder.Entity<StickerPack>(entity =>
            {
                entity.HasKey(sp => sp.Id);

                entity.HasIndex(sp => sp.IsActive);
                entity.HasIndex(sp => sp.IsOfficial);

                entity.HasOne(sp => sp.CoverMedia)
                    .WithMany()
                    .HasForeignKey(sp => sp.CoverMediaId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ===================== STICKER =====================
            modelBuilder.Entity<Sticker>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasIndex(s => s.StickerPackId);
                entity.HasIndex(s => s.MediaId);
                entity.HasIndex(s => new { s.StickerPackId, s.Order }).IsUnique();

                entity.HasOne(s => s.StickerPack)
                    .WithMany(sp => sp.Stickers)
                    .HasForeignKey(s => s.StickerPackId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Media)
                    .WithMany()
                    .HasForeignKey(s => s.MediaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== USER STICKER PACK =====================
            modelBuilder.Entity<UserStickerPack>(entity =>
            {
                entity.HasKey(usp => new { usp.UserId, usp.StickerPackId });

                entity.HasIndex(usp => usp.UserId);
                entity.HasIndex(usp => usp.StickerPackId);
                entity.HasIndex(usp => usp.AddedAt);

                entity.HasOne(usp => usp.User)
                    .WithMany()
                    .HasForeignKey(usp => usp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(usp => usp.StickerPack)
                    .WithMany()
                    .HasForeignKey(usp => usp.StickerPackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== USER RECENT STICKER =====================
            modelBuilder.Entity<UserRecentSticker>(entity =>
            {
                entity.HasKey(urs => new { urs.UserId, urs.StickerId });

                entity.HasIndex(urs => urs.UserId);
                entity.HasIndex(urs => urs.StickerId);
                entity.HasIndex(urs => urs.LastUsedAt).IsDescending();

                entity.HasOne(urs => urs.User)
                    .WithMany()
                    .HasForeignKey(urs => urs.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(urs => urs.Sticker)
                    .WithMany()
                    .HasForeignKey(urs => urs.StickerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CONTACT =====================
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasIndex(c => new { c.UserId, c.ContactUserId }).IsUnique();

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Contacts)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.ContactUser)
                    .WithMany()
                    .HasForeignKey(c => c.ContactUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== BLOCKED USER =====================
            modelBuilder.Entity<BlockedUser>(entity =>
            {
                entity.HasIndex(b => new { b.UserId, b.BlockedUserId }).IsUnique();

                entity.HasOne(b => b.User)
                    .WithMany(u => u.BlockedUsers)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Blocked)
                    .WithMany(u => u.BlockedByUsers)
                    .HasForeignKey(b => b.BlockedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== MEDIA =====================
            modelBuilder.Entity<Media>(entity =>
            {
                entity.HasIndex(m => m.CreatedAt);

                entity.HasIndex(m => m.ContentHash);

                entity.HasIndex(m => m.UploadedByUserId);

                // Configure MediaMetadata as owned value object
                entity.OwnsOne(m => m.Metadata, metadata =>
                {
                    metadata.Property(md => md.Width);
                    metadata.Property(md => md.Height);
                    metadata.Property(md => md.Duration);
                    metadata.Property(md => md.ThumbnailUrl);
                    metadata.Property(md => md.OriginalFileName);
                });

                entity.HasMany(m => m.PostAttachments)
                    .WithOne(pa => pa.Media)
                    .HasForeignKey(pa => pa.MediaId);

                entity.HasMany(m => m.MessageAttachments)
                    .WithOne(ma => ma.Media)
                    .HasForeignKey(ma => ma.MediaId);
            });

            // ===================== MEDIA MODERATION =====================
            modelBuilder.Entity<MediaModeration>(entity =>
            {
                entity.HasOne(mm => mm.Media)
                    .WithMany(m => m.ModerationFlags)
                    .HasForeignKey(mm => mm.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mm => mm.ReviewedBy)
                    .WithMany()
                    .HasForeignKey(mm => mm.ReviewedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===================== MESSAGE ATTACHMENT =====================
            modelBuilder.Entity<MessageAttachment>(entity =>
            {
                entity.HasIndex(x => x.MediaId);

                entity.HasKey(x => new { x.MessageId, x.MediaId });

                entity.HasOne(ma => ma.Message)
                    .WithMany(m => m.MessageAttachments)
                    .HasForeignKey(ma => ma.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ma => ma.Media)
                    .WithMany(m => m.MessageAttachments)
                    .HasForeignKey(ma => ma.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== POST =====================
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(p => new { p.UserId, p.CreatedAt });

                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.CreatedAt);

                entity.HasIndex(p => new { p.UserId, p.Sequence });

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            // ===================== POST ATTACHMENT =====================
            modelBuilder.Entity<PostAttachment>(entity =>
            {
                entity.HasKey(x => new { x.PostId, x.MediaId });

                entity.HasIndex(x => x.MediaId);

                entity.HasOne(pa => pa.Post)
                    .WithMany(p => p.PostAttachments)
                    .HasForeignKey(pa => pa.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pa => pa.Media)
                    .WithMany(m => m.PostAttachments)
                    .HasForeignKey(pa => pa.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== POST LIKE =====================
            modelBuilder.Entity<PostLike>(entity =>
            {
                entity.HasKey(x => new { x.PostId, x.UserId });

                entity.HasIndex(x => x.PostId);

                entity.HasIndex(x => x.UserId);

                entity.HasOne(pl => pl.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(pl => pl.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pl => pl.User)
                    .WithMany()
                    .HasForeignKey(pl => pl.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== STORY =====================
            modelBuilder.Entity<Story>(entity =>
            {
                entity.HasIndex(s => new { s.UserId, s.CreatedAt });
                entity.HasIndex(s => new { s.UserId, s.ExpiresAt });
                entity.HasIndex(s => s.ExpiresAt);

                entity.HasQueryFilter(m => !m.IsDeleted);

                entity.HasOne(s => s.User)
                    .WithMany()
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Media)
                    .WithOne()
                    .HasForeignKey<Story>(s => s.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);

                
            });

            // ===================== STORY SEEN =====================
            modelBuilder.Entity<StorySeen>(entity =>
            {
                entity.HasKey(x => new { x.StoryId, x.UserId });

                entity.HasIndex(x => x.UserId);

                entity.HasIndex(x => x.StoryId);
                entity.HasIndex(x => x.SeenAt);

                entity.HasOne(ss => ss.Story)
                    .WithMany(s => s.SeenBy)
                    .HasForeignKey(ss => ss.StoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ss => ss.User)
                    .WithMany()
                    .HasForeignKey(ss => ss.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== SENSITIVE CONTENT =====================
            modelBuilder.Entity<SensitiveContentFlag>(entity =>
            {
                entity.HasOne(f => f.Media)
                    .WithMany(m => m.SensitiveContentFlags)
                    .HasForeignKey(f => f.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CHANNEL =====================
            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasOne(c => c.CreatorUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatorUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.UrlHistory)
                    .WithOne(cu => cu.Channel)
                    .HasForeignKey(cu => cu.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CHANNEL USER =====================
            modelBuilder.Entity<ChannelUser>(entity =>
            {
                entity.HasKey(cu => new { cu.ChannelId, cu.UserId });

                entity.HasIndex(cu => cu.UserId);
                entity.HasIndex(cu => cu.ChannelId);

                entity.HasOne(cu => cu.Channel)
                    .WithMany(c => c.Subscribers)
                    .HasForeignKey(cu => cu.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cu => cu.User)
                    .WithMany()
                    .HasForeignKey(cu => cu.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CHANNEL ADMIN =====================
            modelBuilder.Entity<ChannelUserAdmin>(entity =>
            {
                entity.HasKey(cua => cua.ChannelUserId);

                entity.HasOne(cua => cua.ChannelUser)
                    .WithOne(cu => cu.AdminPermissions)
                    .HasForeignKey<ChannelUserAdmin>(cua => cua.ChannelUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(cua => cua.GrantedByUser)
                    .WithMany()
                    .HasForeignKey(cua => cua.GrantedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint on (ChannelUserId, GrantedByUserId) to prevent duplicate grants
                entity.HasIndex(cua => new { cua.ChannelUserId, cua.GrantedByUserId }).IsUnique();
            });

            // ===================== CHANNEL USER BAN =====================
            modelBuilder.Entity<ChannelUserBan>(entity =>
            {
                entity.HasKey(b => new { b.ChannelId, b.UserId });

                entity.HasIndex(b => b.UserId);
                entity.HasIndex(b => b.BannedByUserId);
                entity.HasIndex(b => b.BannedAt);

                entity.HasOne(b => b.Channel)
                    .WithMany(c => c.BannedUsers)
                    .HasForeignKey(b => b.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.User)
                    .WithMany()
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.BannedByUser)
                    .WithMany()
                    .HasForeignKey(b => b.BannedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint on (ChannelId, UserId, BannedByUserId) to track ban hierarchy
                entity.HasIndex(b => new { b.ChannelId, b.UserId, b.BannedByUserId }).IsUnique();
            });

            // ===================== CHANNEL URL =====================
            modelBuilder.Entity<ChannelUrl>(entity =>
            {
                entity.HasKey(cu => cu.Id);

                entity.HasIndex(cu => cu.ChannelId);
                entity.HasIndex(cu => cu.UrlSlug).IsUnique();
                entity.HasIndex(cu => cu.IsActive);

                entity.HasOne(cu => cu.Channel)
                    .WithMany(c => c.UrlHistory)
                    .HasForeignKey(cu => cu.ChannelId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(cu => cu.UrlSlug)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // ===================== AVATAR =====================
            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.HasKey(a => a.Id);

                // Soft delete filter
                entity.HasQueryFilter(a => !a.IsDeleted);

                // Indexes for fast lookup
                entity.HasIndex(a => new { a.OwnerType, a.OwnerId });
                entity.HasIndex(a => a.MediaId);
                entity.HasIndex(a => a.UploadedByUserId);

                // Enforce ONLY ONE active avatar per owner
                entity.HasIndex(a => new { a.OwnerType, a.OwnerId, a.IsActive })
                .IsUnique();

                // Relationships

                // Avatar → Media (1-to-1-ish but technically many avatars could reuse media if you allow it)
                entity.HasOne(a => a.Media)
                    .WithMany()
                    .HasForeignKey(a => a.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Avatar → UploadedByUser
                entity.HasOne(a => a.UploadedByUser)
                    .WithMany()
                    .HasForeignKey(a => a.UploadedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Properties
                entity.Property(a => a.OwnerType)
                    .IsRequired();

                entity.Property(a => a.Version)
                    .IsRequired();

                entity.Property(a => a.IsActive)
                    .IsRequired();

                entity.Property(a => a.IsDeleted)
                    .IsRequired();

                entity.Property(a => a.CreatedAt)
                    .IsRequired();
            });

            // ===================== Notifications =====================
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasQueryFilter(n => !n.IsDeleted);

                entity.HasIndex(n => new { n.UserId, n.IsRead });
                entity.HasIndex(n => n.CreatedAt);
                entity.HasIndex(n => n.Priority);
                entity.HasIndex(n => n.GroupingId);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== GIF METADATA =====================
            modelBuilder.Entity<GifMetadata>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.HasIndex(g => g.MediaId).IsUnique();
                entity.HasIndex(g => g.IsTrending);
                entity.HasIndex(g => g.UsageCount);
                entity.HasIndex(g => g.CreatedAt);
                entity.HasIndex(g => g.IsDeleted);

                // One-to-one relationship with Media (for GIF files specifically)
                entity.HasOne(g => g.Media)
                    .WithOne(m => m.GifMetadata)
                    .HasForeignKey<GifMetadata>(g => g.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure Tags as a collection of strings (JSON array in PostgreSQL)
                entity.Property(g => g.Tags)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    .HasMaxLength(1000);

                entity.Property(g => g.EmojiHint).HasMaxLength(10);
                entity.Property(g => g.Title).HasMaxLength(255);
            });

            // ===================== USER RECENT GIF =====================
            modelBuilder.Entity<UserRecentGif>(entity =>
            {
                entity.HasKey(urm => new { urm.UserId, urm.MediaId });

                entity.HasIndex(urm => urm.UserId);
                entity.HasIndex(urm => urm.MediaId);
                entity.HasIndex(urm => urm.LastUsedAt).IsDescending();

                // Relationship to User
                entity.HasOne(urm => urm.User)
                    .WithMany(u => u.UserRecentGifs)
                    .HasForeignKey(urm => urm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship to Media (the GIF file)
                entity.HasOne(urm => urm.Media)
                    .WithMany(m => m.UserRecentGifs)
                    .HasForeignKey(urm => urm.MediaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
