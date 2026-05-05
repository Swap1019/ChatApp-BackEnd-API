using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversation.BanUser
{
    public sealed class BanUserAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public BanUserAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<BanUserAuthorizationData> ExecuteAsync(
            BanUserPolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.CreatedById,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);
            
            // Early exit only on fatal conditions that make the operation invalid
            if (conversation == null || conversation.IsDeleted || !conversation.IsGroup)
            {
                return new BanUserAuthorizationData(
                    null,
                    null,
                    null,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false);
            }

            var actor = await _context.Users
                .Where(u => u.Id == request.ActorId)
                .Select(u => new UserSnapshot(
                    u.IsBanned,
                    u.IsSuspended))
                .FirstOrDefaultAsync(cancellationToken);

            // Check if target user is a member (membership is the gate)
            var isTargetUserMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.TargetUserId,
                    cancellationToken);

            // Fetch actor admin info (only exists if they're admin)
            var actorAdmin = await _context.ConversationUserAdmins
                .Where(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    cua.ConversationUser.UserId == request.ActorId)
                .Select(cua => new AdminSnapshot(
                    cua.ConversationUser.UserId,
                    cua.CanKickMembers))
                .FirstOrDefaultAsync(cancellationToken);

            bool isActorAdmin = actorAdmin != null;
            bool isActorOwner = conversation.CreatedById == request.ActorId;
            bool isTargetUserAdmin = await _context.ConversationUserAdmins
                .AnyAsync(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    cua.ConversationUser.UserId == request.TargetUserId,
                    cancellationToken);
            bool isTargetUserOwner = conversation.CreatedById == request.TargetUserId;
            bool isActorBanned = actor?.IsBanned ?? false;
            bool isActorSuspended = actor?.IsSuspended ?? false;
            bool canBanUsers = actorAdmin?.CanKickMembers ?? false;

            return new BanUserAuthorizationData(
                actorAdmin,
                conversation,
                null,
                isActorAdmin,
                isActorOwner,
                isTargetUserMember,
                isTargetUserAdmin,
                isTargetUserOwner,
                isActorBanned,
                isActorSuspended,
                canBanUsers);
        }
    }
}

