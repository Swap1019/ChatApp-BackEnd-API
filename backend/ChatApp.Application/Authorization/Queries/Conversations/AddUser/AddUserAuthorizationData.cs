using ChatApp.Domain.Enums;

namespace ChatApp.Application.Authorization.Queries.Conversations.AddUser
{
    public sealed record AddUserAuthorizationData(
        AdminSnapshot? ActorUser,
        ConversationSnapshot? Conversation,
        UserSnapshot? TargetUser,
        PrivacyLevel TargetUserAllowsInvites,
        bool IsTargetUserMember,
        bool IsActorAdmin,
        bool IsActorOwner,
        bool IsActorInTargetUserContacts,
        bool IsActorInTargetUserExceptionList,
        bool CanAddUsers)
    {
        public bool IsGroupConversation => Conversation?.IsGroup == true;
    }
}