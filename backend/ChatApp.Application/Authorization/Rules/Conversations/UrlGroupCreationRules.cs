using ChatApp.Application.Authorization.Queries.Conversations.UrlGroupCreation;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class UrlGroupCreationRules
{
    public string? GetFailureReason(UrlGroupCreationAuthorizationData data)
    {
        if (data.Conversation == null || data.Conversation.IsDeleted || data.Conversation.IsGroup)
            return "Conversation not found";
        
        if (!data.IsActorOwner)
            return "Only the conversation owner can create a URL group conversation";

        return null;
    }
}
