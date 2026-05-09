namespace ChatApp.Application.Authorization.Queries.Conversations.UrlGroupCreation
{
    public sealed record UrlGroupCreationAuthorizationData(
    ConversationSnapshot? Conversation,
    bool IsActorOwner)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}

