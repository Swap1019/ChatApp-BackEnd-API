using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Abstractions.Time;
using ChatApp.Application.Authorization.Policies.Messages;
using ChatApp.Application.Authorization.Policies.Messages.SendMessage;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Messages.SendMessage
{
    public sealed class SendMessageAuthorizationQuery
    {
        private readonly IAppDbContext _context;
        private readonly IClock _clock;

        public SendMessageAuthorizationQuery(IAppDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<SendMessageAuthorizationData> ExecuteAsync(
            SendMessagePolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserSnapshot(
                    u.IsBanned,
                    u.IsSuspended))
                .FirstOrDefaultAsync(cancellationToken);

            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            bool isGroupConversation = conversation?.IsGroup == true;

            var targetUserExists = isGroupConversation ||
                await _context.Users.AnyAsync(u => u.Id == request.TargetUserId, cancellationToken);

            var isSenderMember = false;
            var isSenderBanned = false;
            var isBlockedBySender = false;
            var isBlockedByTarget = false;

            if (conversation != null && isGroupConversation)
            {
                isSenderMember = await _context.ConversationUsers
                    .AnyAsync(cu =>
                        cu.ConversationId == request.ConversationId &&
                        cu.UserId == request.UserId,
                        cancellationToken);

                isSenderBanned = await _context.ConversationUserBans
                    .IgnoreQueryFilters()
                    .AnyAsync(b =>
                        b.ConversationId == request.ConversationId &&
                        b.UserId == request.UserId &&
                        !b.IsRevoked &&
                        (b.ExpiresAt == null || b.ExpiresAt > _clock.UtcNow),
                        cancellationToken);
            }

            if (conversation != null && !isGroupConversation)
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

            return new SendMessageAuthorizationData(
                user,
                conversation,
                targetUserExists,
                isSenderMember,
                isSenderBanned,
                isBlockedBySender,
                isBlockedByTarget);
        }
    }
}
