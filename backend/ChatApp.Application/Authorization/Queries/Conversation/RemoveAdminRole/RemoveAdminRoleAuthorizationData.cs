namespace ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole
{
    public sealed record RemoveAdminRoleAuthorizationData(
        AdminSnapshot? ActorAdmin,
        ConversationSnapshot? Conversation,
        AdminSnapshot? TargetAdmin,
        bool IsSelfRemoval,
        bool IsActorActive,
        bool IsActorOwner,
        bool IsTargetOwner,
        bool IsActorMember,
        bool IsTargetMember,
        bool IsActorAdmin,
        bool IsTargetAdmin,
        bool IsConversationBanned)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}
