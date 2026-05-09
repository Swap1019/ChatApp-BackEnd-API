namespace ChatApp.Application.Authorization.Policies.Messages.PinMessage;

public class PinMessagePolicyRequest
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid TargetUserId { get; set; }
}

