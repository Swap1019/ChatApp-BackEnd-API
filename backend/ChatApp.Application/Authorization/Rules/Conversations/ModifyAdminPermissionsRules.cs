using ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions;

namespace ChatApp.Application.Authorization.Rules.Conversations;

public sealed class ModifyAdminPermissionsRules
{
    public string? GetFailureReason(ModifyAdminPermissionsAuthorizationData data)
    {
        if (data.Conversation == null)
            return "Conversation not found";

        if (!data.IsGroupConversation)
            return "Admin roles can only be modified in group conversations";

        if (!data.IsActorOwner && !data.IsActorAdmin)
            return "Only administrators can modify admin roles";

        if (!data.IsActorOwner && !data.CanManageRoles)
            return "You do not have permission to manage admin roles";

        if (data.TargetUser == null)
            return "Target user not found";

        if (!data.IsTargetUserMember)
            return "Target user is not a member of this conversation";

        if (data.IsTargetUserOwner)
            return "You cannot modify roles for the conversation owner";

        if (data.IsTargetUserAdmin && !data.IsGrantedByActor && !data.IsActorOwner)
            return "You cannot modify admin role for users you did not grant admin to, unless you are the owner";

        return null;
    }
}
