namespace ChatApp.Application.Authorization.Policies.Message 
{
    public class SendMessagePolicyRequest
    {
        public Guid UserId { get; set; }
        public Guid TargetUserId { get; set; } // For direct messages, null for group messages
        public Guid ConversationId { get; set; }
    }
}
