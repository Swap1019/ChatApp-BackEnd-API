namespace ChatApp.Application.Authorization.Queries.Conversations.DeleteConversation
{
    public sealed record DeleteConversationAuthorizationData(
        ConversationSnapshot? Conversation,
        UserSnapshot? ActorUser,
        bool IsActorOwner,
        bool IsConversationDeleted,
        bool IsConversationGroup
    )
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}