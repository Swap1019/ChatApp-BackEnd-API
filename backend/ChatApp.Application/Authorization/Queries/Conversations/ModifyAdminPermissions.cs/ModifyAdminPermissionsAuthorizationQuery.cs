using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversations.ModifyAdminPermissions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions
{
    public sealed class ModifyAdminPermissionsAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public ModifyAdminPermissionsAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ModifyAdminPermissionsAuthorizationData> ExecuteAsync(
            ModifyAdminPermissionsPolicyRequest request,
            CancellationToken cancellationToken = default)
        {
        
        var conversation = await _context.Conversations
            .Where(c => c.Id == request.ConversationId)
            .Select(c => new ConversationSnapshot(
                c.CreatedById,
                c.IsGroup,
                c.IsDeleted))
            .FirstOrDefaultAsync(cancellationToken);

            var memberships = await _context.ConversationUsers
                .Where(cu =>
                    cu.ConversationId == request.ConversationId &&
                    (cu.UserId == request.ActorId || cu.UserId == request.TargetUserId))
                .Select(cu => cu.UserId)
                .ToListAsync(cancellationToken);

            var admins = await _context.ConversationUserAdmins
                .Where(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    (cua.ConversationUser.UserId == request.ActorId ||
                    cua.ConversationUser.UserId == request.TargetUserId))
                .Select(cua => new AdminSnapshot(
                    cua.ConversationUser.UserId,
                    cua.GrantedByUserId,
                    cua.CanManageRoles))
                .ToListAsync(cancellationToken);

            var actorUser = admins.FirstOrDefault(cu => cu.UserId == request.ActorId);
            var targetUserAdmin = admins.FirstOrDefault(cu => cu.UserId == request.TargetUserId);
            
            bool isActorAdmin = actorUser != null;
            bool isActorOwner = conversation?.CreatedById == request.ActorId;
            bool isTargetUserMember = memberships.Any(id => id == request.TargetUserId);
            bool isTargetUserAdmin = targetUserAdmin != null;
            bool isTargetUserOwner = conversation?.CreatedById == request.TargetUserId;
            bool isGrantedByActor = targetUserAdmin?.GrantedByUserId == request.ActorId;
            bool canManageRoles = actorUser?.CanManageRoles == true;

            return new ModifyAdminPermissionsAuthorizationData(
                actorUser,
                conversation,
                targetUserAdmin,
                isActorAdmin,
                isActorOwner,
                isTargetUserMember,
                isTargetUserAdmin,
                isTargetUserOwner,
                isGrantedByActor,
                canManageRoles
            );
        }
    }
}

