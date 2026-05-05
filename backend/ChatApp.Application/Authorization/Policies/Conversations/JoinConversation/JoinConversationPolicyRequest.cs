namespace ChatApp.Application.Authorization.Policies.Conversation 
{
    public class JoinConversationPolicyRequest
    {
        public Guid UserId { get; set; }
        public Guid ConversationId { get; set; }
    }
}
