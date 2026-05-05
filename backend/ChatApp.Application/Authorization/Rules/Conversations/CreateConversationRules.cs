using ChatApp.Application.Authorization.Queries.Conversation.CreateConversation;

namespace ChatApp.Application.Authorization.Rules.Conversations
{
    public sealed class CreateConversationRules
    {
        public string? GetFailureReason(CreateConversationAuthorizationData data)
        {
            if (data.ActorUser == null)
                return "Actor user not found";
            
            if (data.ActorUser.IsSuspended)
                return "Actor user is suspended";

            if (data.ActorUser.IsBanned)
                return "Actor user is banned";
            
            if (data.TargetUser != null)
            {
                if (data.TargetUser.IsSuspended)
                    return "Target user is suspended";

                if (data.TargetUser.IsBanned)
                    return "Target user is banned";

                if (data.isBlockedByTargetUser == true)
                    return "You are blocked by the target user";

                if (data.isBlockedByActorUser == true)
                    return "You have blocked the target user";
                
                if (data.Conversation != null)
                    return "A conversation between the actor and target user already exists";
            }

            return null;
        }
    }
}
