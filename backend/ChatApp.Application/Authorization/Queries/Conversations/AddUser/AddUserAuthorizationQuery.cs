using ChatApp.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using ChatApp.Domain.Enums;
using ChatApp.Application.Authorization.Policies.Conversations.AddUser;

namespace ChatApp.Application.Authorization.Queries.Conversations.AddUser
{
    public sealed class AddUserAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public AddUserAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<AddUserAuthorizationData> ExecuteAsync(
            AddUserPolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.CreatedById,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            var actor = await _context.Users
                .Where(u => u.Id == request.ActorId)
                .Select(u => new UserSnapshot(
                    u.Id,
                    u.IsBanned,
                    u.IsSuspended
                    ))
                .FirstOrDefaultAsync(cancellationToken);

            // Fetch target user to validate existence and status
            var targetUser = await _context.Users
                .Where(u => u.Id == request.TargetUserId)
                .Select(u => new UserSnapshot(
                    u.Id,
                    u.IsBanned,
                    u.IsSuspended
                    ))
                .FirstOrDefaultAsync(cancellationToken);

            // Check if target user is a member (membership is the gate)
            var isTargetUserMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.TargetUserId,
                    cancellationToken);
            
            PrivacyLevel targetUserAllowsInvites = await _context.UserPrivacies
                .Where(up => up.UserId == request.TargetUserId)
                .Select(up => up.GroupInvitationsPrivacy)
                .FirstOrDefaultAsync(cancellationToken);

            // Fetch actor admin info (only exists if they're admin)
            var actorAdmin = await _context.ConversationUserAdmins
                .Where(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    cua.ConversationUser.UserId == request.ActorId)
                .Select(cua => new AdminSnapshot(
                    cua.ConversationUser.UserId,
                    cua.CanAddMembers))
                .FirstOrDefaultAsync(cancellationToken);
            
            var targetUserContact = await _context.Contacts
                .Where(c => c.UserId == request.TargetUserId && c.ContactUserId == request.ActorId)
                .FirstOrDefaultAsync(cancellationToken);
            
            var targetUserException = await _context.UserPrivacyExceptions
                .Where(ue => ue.OwnerUserId == request.TargetUserId && ue.TargetUserId == request.ActorId)
                .FirstOrDefaultAsync(cancellationToken);

            bool isActorAdmin = actorAdmin != null;
            bool isActorOwner = conversation?.CreatedById == request.ActorId;
            bool isActorInTargetUserContacts = targetUserContact != null;
            bool isActorInTargetUserExceptionList = targetUserException != null;
            bool canAddUsers = actorAdmin?.CanAddMembers ?? false;

            return new AddUserAuthorizationData(
                actorAdmin,
                conversation,
                targetUser,
                targetUserAllowsInvites,
                isTargetUserMember,
                isActorAdmin,
                isActorOwner,
                isActorInTargetUserContacts,
                isActorInTargetUserExceptionList,
                canAddUsers);
        }
    }
}

