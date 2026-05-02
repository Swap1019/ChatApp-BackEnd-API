namespace ChatApp.Domain.Enums
{
    public enum MessageContextType
    {
        DirectMessage,        // 1-to-1 chat
        GroupMessage,         // group conversation
        ChannelPost,          // broadcast post
        ChannelDiscussion,    // post in a channel that allows comments
    }
}