using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Abstractions.Time;
using ChatApp.Application.Authorization.Policies.Messages;
using ChatApp.Application.Authorization.Policies.Messages.PinMessage;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Messages.PinMessage
{
    public sealed class PinMessageAuthorizationQuery
    {
        private readonly IAppDbContext _context;
        private readonly IClock _clock;

        public PinMessageAuthorizationQuery(IAppDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<PinMessageAuthorizationData> ExecuteAsync(
            PinMessagePolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserSnapshot(
                    u.Id,
                    u.IsBanned,
                    u.IsSuspended))
                .FirstOrDefaultAsync(cancellationToken);

            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.CreatedById,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            var isActorMember = false;
            var isActorAdmin = false;
            var isBlockedBySender = false;
            var isBlockedByTarget = false;
            var canPinMessages = false;

            isActorMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.UserId,
                    cancellationToken);

            if (conversation != null && conversation.IsGroup && !conversation.IsDeleted && user != null)
            {
                var admin = await _context.ConversationUserAdmins
                    .Where(a =>
                        a.ConversationUser.ConversationId == request.ConversationId &&
                        a.ConversationUser.UserId == request.UserId)
                    .Select(a => new AdminSnapshot(
                        a.CanPinMessages))
                    .FirstOrDefaultAsync(cancellationToken);

                isActorAdmin = admin != null;
                canPinMessages = admin?.CanPinMessages ?? false;
            }

            if (conversation != null && !conversation.IsGroup && !conversation.IsDeleted && user != null)
                {
                    isBlockedBySender = await _context.BlockedUsers
                        .AnyAsync(b =>
                            b.UserId == request.UserId &&
                            b.BlockedUserId == request.TargetUserId,
                            cancellationToken);

                    isBlockedByTarget = await _context.BlockedUsers
                        .AnyAsync(b =>
                            b.UserId == request.TargetUserId &&
                            b.BlockedUserId == request.UserId,
                            cancellationToken);
                }

            return new PinMessageAuthorizationData(
                user,
                conversation,
                isActorMember,
                isActorAdmin,
                isBlockedBySender,
                isBlockedByTarget,
                canPinMessages);
        }
    }
}

