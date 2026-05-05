using ChatApp.Application.Authorization.Queries.Conversation.JoinConversation;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class JoinConversationRules
{
    public string? GetFailureReason(JoinConversationAuthorizationData data)
    {
        if (data.User == null)
            return "User not found";

        if (!data.IsUserActive)
            return "User is not allowed to perform this action";

        if (data.Conversation == null)
            return "Conversation not found";

        if (!data.IsConversationValid)
            return "Conversation not found";

        if (data.IsUserBanned)
            return "User is banned from this conversation";

        return null;
    }
}
