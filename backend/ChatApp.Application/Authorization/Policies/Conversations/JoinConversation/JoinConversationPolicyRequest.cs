namespace ChatApp.Application.Authorization.Policies.Conversations.JoinConversation;

public class JoinConversationPolicyRequest
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
}