using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversations.UrlGroupCreation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversations.UrlGroupCreation
{
    public sealed class UrlGroupCreationAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public UrlGroupCreationAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<UrlGroupCreationAuthorizationData> ExecuteAsync(
            UrlGroupCreationPolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.CreatedById,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);
            
            var actorAdmin = await _context.ConversationUserAdmins
                .Where(cua =>
                    cua.ConversationUser.ConversationId == request.ConversationId &&
                    cua.ConversationUser.UserId == request.ActorId)
                .FirstOrDefaultAsync(cancellationToken);
            
            bool isActorOwner = conversation?.CreatedById == request.ActorId;
            
            return new UrlGroupCreationAuthorizationData(
                conversation, 
                isActorOwner
                );
        }
    }
}
