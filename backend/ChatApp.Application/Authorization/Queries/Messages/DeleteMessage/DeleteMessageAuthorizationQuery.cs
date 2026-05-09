using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Abstractions.Time;
using ChatApp.Application.Authorization.Policies.Messages;
using ChatApp.Application.Authorization.Policies.Messages.DeleteMessage;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Authorization.Queries.Messages.DeleteMessage
{
    public sealed class DeleteMessageAuthorizationQuery
    {
        private readonly IAppDbContext _context;
        private readonly IClock _clock;

        public DeleteMessageAuthorizationQuery(IAppDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<DeleteMessageAuthorizationData> ExecuteAsync(
            DeleteMessagePolicyRequest request,
            CancellationToken cancellationToken = default)
        {
            
            var conversation = await _context.Conversations
                .Where(c => c.Id == request.ConversationId)
                .Select(c => new ConversationSnapshot(
                    c.Id,
                    c.CreatedById,
                    c.IsGroup,
                    c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);

            var message = await _context.Messages
                .Where(m => m.Id == request.MessageId)
                .Select(m => new MessageSnapshot(
                    m.SenderId,
                    m.ContextType,
                    m.ConversationId,
                    m.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);
            
            bool isActorMember = await _context.ConversationUsers
                .AnyAsync(cu =>
                    cu.ConversationId == request.ConversationId &&
                    cu.UserId == request.ActorId,
                    cancellationToken);
            
            var admin = await _context.ConversationUserAdmins
                .Where(a =>
                    a.ConversationUser.ConversationId == request.ConversationId &&
                    a.ConversationUser.UserId == request.ActorId)
                .FirstOrDefaultAsync(cancellationToken);

            Guid ActorId = request.ActorId;
            bool isActorAdmin = admin != null;
            bool canDeleteMessages = admin?.CanDeleteMessages ?? false;
            bool isActorSender = message?.SenderId == request.ActorId;
            bool isActorOwner = conversation?.CreatedById == request.ActorId; 

            return new DeleteMessageAuthorizationData(
                conversation,
                message,
                ActorId,
                isActorMember,
                isActorAdmin,
                isActorSender,
                isActorOwner,
                canDeleteMessages);
        }
    }
}

