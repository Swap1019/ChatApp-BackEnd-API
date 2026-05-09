namespace ChatApp.Application.Authorization.Queries.Conversations.JoinConversation
{
    public sealed record JoinConversationAuthorizationData(
        UserSnapshot? User,
        ConversationSnapshot? Conversation,
        bool IsUserBanned)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}
