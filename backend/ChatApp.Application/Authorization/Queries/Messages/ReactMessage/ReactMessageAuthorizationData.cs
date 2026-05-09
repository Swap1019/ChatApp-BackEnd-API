namespace ChatApp.Application.Authorization.Queries.Messages.ReactMessage
{
    public sealed record ReactMessageAuthorizationData(
        MessageSnapshot? Message,
        ConversationSnapshot? Conversation,
        bool IsActorMember,
        bool IsActorBanned)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}


