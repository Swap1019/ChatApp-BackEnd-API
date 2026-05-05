using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversation.DeleteConversation
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
            
            // Early exit only on fatal conditions that make the operation invalid
            if (conversation == null)
            {
                return new DeleteConversationAuthorizationData(
                    null,
                    null,
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

            bool isActorOwner = conversation.CreatedById == request.ActorId;
            bool isActorBanned = actor?.IsBanned ?? false;
            bool isActorSuspended = actor?.IsSuspended ?? false;

            return new DeleteConversationAuthorizationData(
                conversation,
                actor,
                isActorBanned,
                isActorSuspended,
                isActorOwner,
                conversation.IsDeleted,
                conversation.IsGroup);
        }
    }
}

