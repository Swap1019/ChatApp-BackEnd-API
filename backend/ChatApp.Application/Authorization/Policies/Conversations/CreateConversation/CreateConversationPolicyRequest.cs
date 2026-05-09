namespace ChatApp.Application.Authorization.Policies.Conversations.CreateConversation;

public class CreateConversationPolicyRequest
{
    public Guid ActorId { get; set; }
    public Guid? TargetUserId { get; set; }

}