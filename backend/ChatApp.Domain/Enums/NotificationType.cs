namespace ChatApp.Domain.Enums
{
    public enum NotificationType
    {
        MessageReceived,
        MessageReaction,
        MessageReply,
        ConversationInvite,
        ConversationRemoved,
        PostLike,
        PostComment,
        StoryMention,
        FriendRequest,
        SystemAlert
    }
}