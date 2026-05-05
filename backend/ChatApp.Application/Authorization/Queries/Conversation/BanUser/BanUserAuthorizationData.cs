namespace ChatApp.Application.Authorization.Queries.Conversation.BanUser
{
    public sealed record BanUserAuthorizationData(
        AdminSnapshot? ActorUser,
        ConversationSnapshot? Conversation,
        dynamic? TargetUser,
        bool IsActorAdmin,
        bool IsActorOwner,
        bool IsTargetUserMember,
        bool IsTargetUserAdmin,
        bool IsTargetUserOwner,
        bool IsActorBanned,
        bool IsActorSuspended,
        bool CanBanUsers)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}