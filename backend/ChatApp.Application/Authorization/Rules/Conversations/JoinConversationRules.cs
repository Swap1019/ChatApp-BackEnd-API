using ChatApp.Application.Authorization.Queries.Conversations.JoinConversation;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class JoinConversationRules
{
    public string? GetFailureReason(JoinConversationAuthorizationData data)
    {
        if (data.Conversation == null || data.Conversation.IsDeleted || !data.Conversation.IsGroup)
            return "Conversation not found";

        if (data.IsUserBanned)
            return "User is banned from this conversation";

        return null;
    }
}
