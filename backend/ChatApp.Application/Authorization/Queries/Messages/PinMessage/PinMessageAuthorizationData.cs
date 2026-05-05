namespace ChatApp.Application.Authorization.Queries.Messages.PinMessage
{
    public sealed record PinMessageAuthorizationData(
        UserSnapshot? User,
        ConversationSnapshot? Conversation,
        bool IsActorMember,
        bool IsActorBanned,
        bool IsActorAdmin,
        bool IsBlockedBySender,
        bool IsBlockedByTarget,
        bool CanPinMessages)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}


