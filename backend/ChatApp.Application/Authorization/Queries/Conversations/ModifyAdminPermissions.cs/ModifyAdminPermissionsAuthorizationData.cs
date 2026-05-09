using ConversationEntites = ChatApp.Domain.Entities.Conversation;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions
{
    public sealed record ModifyAdminPermissionsAuthorizationData(
    AdminSnapshot? ActorUser,
    ConversationSnapshot? Conversation,
    AdminSnapshot? TargetUser,
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