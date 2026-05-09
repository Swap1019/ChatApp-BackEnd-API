namespace ChatApp.Application.Authorization.Queries.Conversations.RemoveAdminRole
{
    public sealed record RemoveAdminRoleAuthorizationData(
        AdminSnapshot? ActorAdmin,
        ConversationSnapshot? Conversation,
        AdminSnapshot? TargetAdmin,
        bool IsSelfRemoval,
        bool IsActorOwner,
        bool IsTargetOwner,
        bool IsActorMember,
        bool IsTargetMember,
        bool IsActorAdmin,
        bool IsTargetAdmin)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}

