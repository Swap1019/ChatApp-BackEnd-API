namespace ChatApp.Application.Authorization.Queries.Conversation.CreateConversation
{
    public sealed record CreateConversationAuthorizationData(
        ConversationSnapshot? Conversation,
        UserSnapshot ActorUser,
        UserSnapshot? TargetUser,
        bool IsActorBanned,
        bool IsActorSuspended,
        bool isBlockedByTargetUser,
        bool isBlockedByActorUser,
        bool? IsTargetUserBanned,
        bool? IsTargetUserSuspended)
    {
    }
}