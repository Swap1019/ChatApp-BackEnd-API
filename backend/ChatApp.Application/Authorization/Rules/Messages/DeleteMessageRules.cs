using ChatApp.Application.Authorization.Queries.Messages.DeleteMessage;
using ChatApp.Domain.Entities.Messaging;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Authorization.Rules.Messages;

public sealed class DeleteMessageRules
{
    public string? GetFailureReason(DeleteMessageAuthorizationData data)
    {
        if (data.Conversation == null || data.Conversation.IsDeleted)
            return "Conversation not found";
        
        if (!data.IsActorMember)
            return "User is not a member of this conversation";
        
        if (data.Message == null)
            return "Message not found";
        
        if (data.Message.IsDeleted)
            return "Message already deleted";

        if (data.Message.ConversationId != data.Conversation.Id)
            return "Message does not belong to this conversation";

        if (data.Message.ContextType == MessageContextType.ChannelPost)
            return "Channel post messages cannot be deleted here";
        
        if (data.Conversation.IsGroup)
        {
            var isSender = data.Message.SenderId == data.ActorId;

            if (isSender)
                return null;

            if (!data.IsActorAdmin && !data.IsActorOwner)
                return "You are not allowed to delete this message";

            if (!data.CanDeleteMessages && !data.IsActorOwner)
                return "You don't have permission to delete this message";
        }

        return null;
    }
}
