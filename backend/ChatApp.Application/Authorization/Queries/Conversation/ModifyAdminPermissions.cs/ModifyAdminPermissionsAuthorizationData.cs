using ConversationEntites = ChatApp.Domain.Entities.Conversation;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.Authorization.Queries.Conversation.ModifyAdminPermissions
{
    public sealed record ModifyAdminPermissionsAuthorizationData(
    ConversationEntites.ConversationUserAdmin? ActorUser,
    ConversationEntites.Conversation? Conversation,
    ConversationEntites.ConversationUserAdmin? TargetUser,
    bool IsActorBanned,
    bool IsActorSuspended,
    bool IsActorAdmin,
    bool IsActorOwner,
    bool IsTargetUserMember,
    bool IsTargetUserAdmin,
    bool IsTargetUserOwner,
    bool IsGrantedByActor,
    bool CanManageRoles)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}