using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversations.DeleteConversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversations.DeleteConversation
{
    public sealed class DeleteConversationAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public DeleteConversationAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<DeleteConversationAuthorizationData> ExecuteAsync(
            DeleteConversationPolicyRequest request,
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
                    u.IsBanned,
                    u.IsSuspended))
                .FirstOrDefaultAsync(cancellationToken);

            bool isActorOwner = conversation?.CreatedById == request.ActorId;

            return new DeleteConversationAuthorizationData(
                conversation,
                actor,
                isActorOwner,
                conversation?.IsDeleted ?? false,
                conversation?.IsGroup ?? false);
        }
    }
}

