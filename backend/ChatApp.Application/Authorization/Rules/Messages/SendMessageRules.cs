using ChatApp.Application.Authorization.Queries.Messages.SendMessage;

namespace ChatApp.Application.Authorization.Rules.Messages;

public sealed class SendMessageRules
{
    public string? GetFailureReason(SendMessageAuthorizationData data)
    {
        if (data.User == null)
            return "User not found";

        if (data.User.IsBanned || data.User.IsSuspended)
            return "User is not allowed to perform this action";

        if (data.Conversation == null)
            return "Conversation not found";

        if (data.Conversation.IsDeleted)
            return "Conversation is deleted";

        if (!data.TargetUserExists && !data.IsGroupConversation)
            return "Target user not found";

        if (data.IsGroupConversation)
        {
            if (!data.IsSenderMember)
                return "User is not a member of this conversation";

            if (data.IsSenderBanned)
                return "User is banned from this conversation";
        }
        else if (data.IsBlockedBySender || data.IsBlockedByTarget)
        {
            return "You cannot send messages to this user";
        }

        return null;
    }
}
