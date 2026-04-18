using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<UserSuspension> UserSuspensions { get; set; }
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
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<Story> Stories { get; set; }
        public DbSet<StorySeen> StorySeens { get; set; }

        public DbSet<Media> Media { get; set; }
        public DbSet<MediaModeration> MediaModerations { get; set; }
        public DbSet<MessageAttachment> MessageAttachments { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }

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
                entity.HasOne(cua => cua.ConversationUser)
                    .WithMany(cu => cu.AdminPermissions)
                    .HasForeignKey(cua => cua.ConversationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===================== CONVERSATION USER BAN =====================
            modelBuilder.Entity<ConversationUserBan>(entity =>
            {
                entity.HasKey(b => new { b.ConversationId, b.UserId });

                entity.HasIndex(b => b.UserId);
                entity.HasIndex(b => b.BannedByUserId);

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

                entity.Property(b => b.BannedAt)
                    .IsRequired();
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

                entity.HasOne(m => m.ForwardedToConversation)
                    .WithMany()
                    .HasForeignKey(m => m.ForwardedToConversationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Thread)
                    .WithMany(t => t.Messages)
                    .HasForeignKey(m => m.ThreadId)
                    .OnDelete(DeleteBehavior.Restrict);
                
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
        }
    }
}