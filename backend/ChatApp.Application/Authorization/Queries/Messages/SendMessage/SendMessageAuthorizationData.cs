namespace ChatApp.Application.Authorization.Queries.Messages.SendMessage
{
    public sealed record SendMessageAuthorizationData(
        UserSnapshot? User,
        ConversationSnapshot? Conversation,
        bool TargetUserExists,
        bool IsSenderMember,
        bool IsSenderBanned,
        bool IsBlockedBySender,
        bool IsBlockedByTarget)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}


