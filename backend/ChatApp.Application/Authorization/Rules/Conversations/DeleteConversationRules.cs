using ChatApp.Application.Authorization.Queries.Conversations.DeleteConversation;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class DeleteConversationRules
{
    public string? GetFailureReason(DeleteConversationAuthorizationData data)
    {
        if (data.ActorUser == null)
            return "Actor user not found";
        
        if (data.ActorUser.IsSuspended)
            return "Actor user is suspended";

        if (data.ActorUser.IsBanned)
            return "Actor user is banned";
        
        if (data.Conversation == null)
            return "No Conversation found with the given id";
        
        if (data.IsConversationDeleted)
            return "No Conversation found with the given id";
        
        if (data.IsConversationGroup)
        {
            if (!data.IsActorOwner)
                return "Only the conversation owner can delete a group conversation";
        }

        return null;
    }
}
