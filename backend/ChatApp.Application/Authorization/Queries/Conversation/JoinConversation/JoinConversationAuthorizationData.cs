namespace ChatApp.Application.Authorization.Queries.Conversation.JoinConversation
{
    public sealed record JoinConversationAuthorizationData(
        UserSnapshot? User,
        ConversationSnapshot? Conversation,
        bool IsUserActive,
        bool IsConversationValid,
        bool IsUserBanned)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}
