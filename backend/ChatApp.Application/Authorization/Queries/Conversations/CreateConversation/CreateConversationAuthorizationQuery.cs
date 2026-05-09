using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversations.CreateConversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversations.CreateConversation
{
    public sealed class CreateConversationAuthorizationQuery
    {
        private readonly IAppDbContext _context;

        public CreateConversationAuthorizationQuery(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateConversationAuthorizationData> ExecuteAsync(
            CreateConversationPolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            var actorUser = await _context.Users
                .Where(u => u.Id == request.ActorId)
                .Select(u => new UserSnapshot(
                    u.IsBanned,
                    u.IsSuspended))
                .FirstOrDefaultAsync(cancellationToken);

            UserSnapshot? targetUser = null;
            ConversationSnapshot? conversation = null;
            bool isBlockedByTargetUser = false;
            bool isBlockedByActorUser = false;

            if (request.TargetUserId.HasValue)
            {
                targetUser = await _context.Users
                    .Where(u => u.Id == request.TargetUserId.Value)
                    .Select(u => new UserSnapshot(
                        u.IsBanned,
                        u.IsSuspended))
                    .FirstOrDefaultAsync(cancellationToken);
                    
                conversation = await _context.Conversations
                    .Where(c =>
                        c.IsGroup == false &&
                        c.Members.Any(m => m.UserId == request.ActorId) &&
                        c.Members.Any(m => m.UserId == request.TargetUserId.Value))
                    .Select(c => new ConversationSnapshot(
                        c.Id,
                        c.IsGroup))
                    .FirstOrDefaultAsync(cancellationToken);
                
                isBlockedByTargetUser = await _context.BlockedUsers
                    .AnyAsync(bu =>
                        bu.UserId == request.TargetUserId.Value &&
                        bu.BlockedUserId == request.ActorId,
                        cancellationToken);
                
                isBlockedByActorUser = await _context.BlockedUsers
                    .AnyAsync(bu =>
                        bu.UserId == request.ActorId &&
                        bu.BlockedUserId == request.TargetUserId.Value,
                        cancellationToken);
                
            }

            return new CreateConversationAuthorizationData(
                Conversation: conversation,
                ActorUser: actorUser,
                TargetUser: targetUser,
                isBlockedByTargetUser: isBlockedByTargetUser,
                isBlockedByActorUser: isBlockedByActorUser);
        }
    }
}

