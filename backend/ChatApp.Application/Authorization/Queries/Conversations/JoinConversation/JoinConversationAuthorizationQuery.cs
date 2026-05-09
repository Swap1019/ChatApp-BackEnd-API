using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversations.JoinConversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversations.JoinConversation
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

            return new JoinConversationAuthorizationData(
                user,
                conversation,
                isBanned);
        }
    }
}
