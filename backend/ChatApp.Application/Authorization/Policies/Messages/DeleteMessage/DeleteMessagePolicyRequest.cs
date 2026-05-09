namespace ChatApp.Application.Authorization.Policies.Messages.DeleteMessage;

public class DeleteMessagePolicyRequest
{
    public Guid ActorId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
}

