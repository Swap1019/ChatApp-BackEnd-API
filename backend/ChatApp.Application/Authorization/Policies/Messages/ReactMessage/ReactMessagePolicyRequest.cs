namespace ChatApp.Application.Authorization.Policies.Messages.ReactMessage;

public class ReactMessagePolicyRequest
{
    public Guid ActorId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
}

