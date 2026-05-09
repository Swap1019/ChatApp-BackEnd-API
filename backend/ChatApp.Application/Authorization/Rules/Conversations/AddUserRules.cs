using ChatApp.Application.Authorization.Queries.Conversations.AddUser;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class AddUserRules
{
    public string? GetFailureReason(AddUserAuthorizationData data)
    {
        if (data.Conversation == null || data.Conversation.IsDeleted || !data.Conversation.IsGroup)
            return "Conversation not found";

        if (data.TargetUser == null)
            return "Target user not found";

        if (data.TargetUser.IsBanned)
            return "Target user is not allowed to receive invites";
            
        if (data.IsTargetUserMember)
            return "Target user is already a member of this conversation";

        if (data.TargetUserAllowsInvites == PrivacyLevel.Nobody)
            return "Target user does not allow invites";

        if (data.TargetUserAllowsInvites == PrivacyLevel.ContactsOnly && !data.IsActorInTargetUserContacts)
            return "Target user does not allow invites";

        if (data.TargetUserAllowsInvites == PrivacyLevel.Custom && !data.IsActorInTargetUserExceptionList)
            return "Target user does not allow invites";
        
        if (!data.IsActorOwner)
        {
            if (!data.IsActorAdmin || !data.CanAddUsers)
                return "Only administrators with permission can add users";
        }

        return null;
    }
}

