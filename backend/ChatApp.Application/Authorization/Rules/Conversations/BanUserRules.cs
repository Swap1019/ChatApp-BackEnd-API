using System.Runtime.Serialization.Formatters;
using ChatApp.Application.Authorization.Queries.Conversation.BanUser;

namespace ChatApp.Application.Authorization.Rules.Conversations
{
    public sealed class BanUserRules
    {
        public string? GetFailureReason(BanUserAuthorizationData data)
        {
            if (data.Conversation == null)
                return "Conversation not found";

            if (data.Conversation.IsDeleted)
                return "Conversation is deleted";
            
            if (data.ActorUser == null)
                return "Actor user not found";

            if (data.TargetUser == null)
                return "Target user not found";

            if (data.IsActorBanned || data.IsActorSuspended)
                return "Actor user is not allowed to perform this action";
            
            if (!data.IsActorAdmin || !data.CanBanUsers)
                return "Only administrators with permission can ban users";
            
            if (!data.IsTargetUserMember)
                return "Target user is not a member of this conversation";
            
            if (data.IsTargetUserOwner)
                return "Cannot ban the conversation owner";
            
            if (data.IsTargetUserAdmin && !data.IsActorOwner)
                return "Only the owner can ban administrators";

            return null;
        }
    }
}
