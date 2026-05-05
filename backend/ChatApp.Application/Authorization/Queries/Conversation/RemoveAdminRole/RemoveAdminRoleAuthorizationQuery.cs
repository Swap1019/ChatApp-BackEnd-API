using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole
{
    public sealed class RemoveAdminRoleAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public RemoveAdminRoleAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<RemoveAdminRoleAuthorizationData> ExecuteAsync(
            RemoveAdminRolePolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            // Self-removal check
            if (request.ActorId == request.TargetUserId)
            {
                return new RemoveAdminRoleAuthorizationData(
                    null,
                    null,
                    null,
                    true,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false);
            }

            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.CreatedById,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            // Early exit only on fatal conditions
            if (conversation == null || conversation.IsDeleted)
            {
                return new RemoveAdminRoleAuthorizationData(
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
                    false,
                    false);
            }

            var actor = await _context.Users
                .Where(u => u.Id == request.ActorId)
                .Select(u => new UserSnapshot(
                    u.IsBanned,
                    u.IsSuspended))
                .FirstOrDefaultAsync(cancellationToken);

            // Check membership for both users
            var actorMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.ActorId,
                    cancellationToken);

            var targetMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.TargetUserId,
                    cancellationToken);

            // Check if target is admin
            var targetAdmin = await _context.ConversationUserAdmins
                .Where(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    cua.ConversationUser.UserId == request.TargetUserId)
                .Select(cua => new AdminSnapshot(
                    cua.ConversationUser.UserId,
                    cua.GrantedByUserId))
                .FirstOrDefaultAsync(cancellationToken);

            // Check if actor is admin
            var actorAdmin = await _context.ConversationUserAdmins
                .Where(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    cua.ConversationUser.UserId == request.ActorId)
                .Select(cua => new AdminSnapshot(
                    cua.ConversationUser.UserId,
                    cua.GrantedByUserId))
                .FirstOrDefaultAsync(cancellationToken);

            // Check if actor is banned
            var isActorBanned = await _context.ConversationUserBans
                .IgnoreQueryFilters()
                .AnyAsync(b =>
                    b.ConversationId == request.ConversationId &&
                    b.UserId == request.ActorId &&
                    !b.IsRevoked,
                    cancellationToken);

            bool isSelfRemoval = request.ActorId == request.TargetUserId;
            bool isActorActive = actor != null && !actor.IsBanned && !actor.IsSuspended;
            bool isActorOwner = conversation.CreatedById == request.ActorId;
            bool isTargetOwner = conversation.CreatedById == request.TargetUserId;
            bool isActorMember = actorMember;
            bool isTargetMember = targetMember;
            bool isActorAdmin = actorAdmin != null;
            bool isTargetAdmin = targetAdmin != null;
            bool isConversationBanned = isActorBanned;

            return new RemoveAdminRoleAuthorizationData(
                actorAdmin,
                conversation,
                targetAdmin,
                isSelfRemoval,
                isActorActive,
                isActorOwner,
                isTargetOwner,
                isActorMember,
                isTargetMember,
                isActorAdmin,
                isTargetAdmin,
                isConversationBanned);
        }
    }
}
