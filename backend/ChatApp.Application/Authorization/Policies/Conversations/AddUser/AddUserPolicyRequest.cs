namespace ChatApp.Application.Authorization.Policies.Conversations.AddUser;

public class AddUserPolicyRequest
{
    public Guid ActorId { get; set; }
    public Guid TargetUserId { get; set; }
    public Guid ConversationId { get; set; }
}