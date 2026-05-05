using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Authorization.Policies.Conversation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Conversation.CreateConversation
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

            // For create conversation, we only check if the users are banned/suspended globally
            // since the conversation doesn't exist yet. If the target user is not found, we treat them as not banned/suspended.
            bool isActorBanned = actorUser?.IsBanned ?? false;
            bool isActorSuspended = actorUser?.IsSuspended ?? false;
            bool? isTargetUserBanned = targetUser != null ? targetUser.IsBanned : (bool?)null;
            bool? isTargetUserSuspended = targetUser != null ? targetUser.IsSuspended : (bool?)null;

            return new CreateConversationAuthorizationData(
                Conversation: conversation,
                ActorUser: actorUser ?? throw new InvalidOperationException("Actor user not found"),
                TargetUser: targetUser,
                IsActorBanned: isActorBanned,
                IsActorSuspended: isActorSuspended,
                isBlockedByTargetUser: isBlockedByTargetUser,
                isBlockedByActorUser: isBlockedByActorUser,
                IsTargetUserBanned: isTargetUserBanned,
                IsTargetUserSuspended: isTargetUserSuspended);
        }
    }
}

