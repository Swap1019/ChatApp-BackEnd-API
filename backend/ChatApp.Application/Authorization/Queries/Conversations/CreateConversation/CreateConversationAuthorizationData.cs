namespace ChatApp.Application.Authorization.Queries.Conversations.CreateConversation
{
    public sealed record CreateConversationAuthorizationData(
        ConversationSnapshot? Conversation,
        UserSnapshot? ActorUser,
        UserSnapshot? TargetUser,
        bool isBlockedByTargetUser,
        bool isBlockedByActorUser)
    {
    }
}