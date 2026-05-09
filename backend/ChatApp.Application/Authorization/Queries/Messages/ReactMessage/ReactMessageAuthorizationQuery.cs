using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Abstractions.Time;
using ChatApp.Application.Authorization.Policies.Messages;
using ChatApp.Application.Authorization.Policies.Messages.ReactMessage;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Messages.ReactMessage
{
    public sealed class ReactMessageAuthorizationQuery
    {
        private readonly IAppDbContext _context;
        private readonly IClock _clock;

        public ReactMessageAuthorizationQuery(IAppDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<ReactMessageAuthorizationData> ExecuteAsync(
            ReactMessagePolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.Id,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            var message = await _context.Messages
                .Where(m => m.Id == request.MessageId)
                .Select(m => new MessageSnapshot(
                    m.Id,
                    m.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);
            
            bool isActorMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.ActorId,
                    cancellationToken);

            bool isActorBanned = await _context.ConversationUserBans
                .IgnoreQueryFilters()
                .AnyAsync(b =>
                    b.ConversationId == request.ConversationId &&
                    b.UserId == request.ActorId &&
                    !b.IsRevoked &&
                    (b.ExpiresAt == null || b.ExpiresAt > _clock.UtcNow),
                    cancellationToken);
            
            return new ReactMessageAuthorizationData(
                message,
                conversation,
                isActorMember,
                isActorBanned);
        }
    }
}

