namespace ChatApp.Application.Authorization.Queries.Messages.DeleteMessage
{
    public sealed record DeleteMessageAuthorizationData(
        ConversationSnapshot? Conversation,
        MessageSnapshot? Message,
        Guid ActorId,
        bool IsActorMember,
        bool IsActorAdmin,
        bool IsActorSender,
        bool IsActorOwner,
        bool CanDeleteMessages)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}


