using ChatApp.Application.Authorization.Queries.Messages.ReactMessage;
using ChatApp.Domain.Entities.Messaging;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Authorization.Rules.Messages;

public sealed class ReactMessageRules
{
    public string? GetFailureReason(ReactMessageAuthorizationData data)
    {
        if (data.Conversation == null || data.Conversation.IsDeleted)
            return "Conversation not found";
        
        if (data.Message == null)
            return "Message not found";
        
        if (data.Message.IsDeleted)
            return "Message already deleted";
        
        if (data.Conversation.IsGroup)
        {
            if (data.IsActorBanned)
                return "You are banned from this conversation";
        }
        else if (!data.Conversation.IsGroup)
        {  
            if (!data.IsActorMember)
                return "You are not a member of this conversation";
        }

        return null;
    }
}
