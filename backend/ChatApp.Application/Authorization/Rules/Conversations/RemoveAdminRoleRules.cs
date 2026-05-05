using ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class RemoveAdminRoleRules
{
    public string? GetFailureReason(RemoveAdminRoleAuthorizationData data)
    {
        if (data.IsSelfRemoval)
            return "You cannot remove admin role from yourself";

        if (data.Conversation == null)
            return "Conversation not found";

        if (data.Conversation.IsDeleted)
            return "Conversation is deleted";

        if (!data.IsActorActive)
            return "Actor is not allowed to perform this action";

        if (!data.IsActorMember)
            return "You are not a member of this conversation";

        if (!data.IsTargetMember)
            return "Target user is not a member of this conversation";

        if (data.IsConversationBanned)
            return "You are banned from this conversation";

        if (data.IsTargetOwner)
            return "You cannot remove admin role from the conversation owner";

        if (!data.IsTargetAdmin)
            return "Target user is not an admin in this conversation";

        if (!data.IsActorAdmin)
            return "You do not have permission to remove admin roles";

        // Prevent non-owner admin from removing another admin
        if (!data.IsActorOwner && data.IsTargetAdmin)
            return "Only the conversation owner can remove admin role from other admins";

        return null;
    }
}
