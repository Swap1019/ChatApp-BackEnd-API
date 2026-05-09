namespace ChatApp.Application.Authorization.Policies.Conversations.DeleteConversation;

public class DeleteConversationPolicyRequest
{
    public Guid ActorId { get; set; }
    public Guid ConversationId { get; set; }

}