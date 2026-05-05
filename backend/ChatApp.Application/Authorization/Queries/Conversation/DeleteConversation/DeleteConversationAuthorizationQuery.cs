namespace ChatApp.Application.Authorization.Queries.Conversation.DeleteConversation
{
    public sealed record DeleteConversationAuthorizationData(
        ConversationSnapshot? Conversation,
        UserSnapshot? ActorUser,
        bool IsActorBanned,
        bool IsActorSuspended,
        bool IsActorOwner,
        bool IsConversationDeleted,
        bool IsConversationGroup
    )
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}