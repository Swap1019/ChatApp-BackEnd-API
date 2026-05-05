using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversation.JoinConversation
{
    public sealed class JoinConversationAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public JoinConversationAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<JoinConversationAuthorizationData> ExecuteAsync(
            JoinConversationPolicyRequest request,
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

            var isBanned = false;

            if (conversation != null)
            {
                isBanned = await _context.ConversationUserBans
                    .IgnoreQueryFilters()
                    .AnyAsync(b =>
                        b.ConversationId == request.ConversationId &&
                        b.UserId == request.UserId &&
                        !b.IsRevoked,
                        cancellationToken);
            }

            bool isUserActive = user != null && !user.IsBanned && !user.IsSuspended;
            bool isConversationValid = conversation != null && !conversation.IsDeleted && conversation.IsGroup;
            bool isUserBanned = isBanned;

            return new JoinConversationAuthorizationData(
                user,
                conversation,
                isUserActive,
                isConversationValid,
                isUserBanned);
        }
    }
}
