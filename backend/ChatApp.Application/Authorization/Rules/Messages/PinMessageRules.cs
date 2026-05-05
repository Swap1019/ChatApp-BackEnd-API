using ChatApp.Application.Authorization.Queries.Messages.PinMessage;

namespace ChatApp.Application.Authorization.Rules.Messages;

public sealed class PinMessageRules
{
    public string? GetFailureReason(PinMessageAuthorizationData data)
    {
        if (data.User == null)
            return "User not found";

        if (data.User.IsBanned || data.User.IsSuspended)
            return "User is not allowed to perform this action";

        if (data.Conversation == null)
            return "Conversation not found";

        if (data.Conversation.IsDeleted)
            return "Conversation is deleted";
        
        if (!data.IsActorMember)
            return "User is not a member of this conversation";
        
        if (data.Conversation.IsGroup)
        {
            if (data.IsActorBanned)
                return "User is banned from this conversation";

            if (data.Conversation.CreatedById != data.User.Id &&
                (!data.IsActorAdmin || !data.CanPinMessages))
            {
                return "You do not have permission to pin messages";
            }
        } else
        {
            if (data.IsBlockedBySender || data.IsBlockedByTarget)
            {
                return "You cannot pin messages in this conversation";
            }
        }

        return null;
    }
}
