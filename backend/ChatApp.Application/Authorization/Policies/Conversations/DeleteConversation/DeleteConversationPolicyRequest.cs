namespace ChatApp.Application.Authorization.Policies.Conversation 
{
    public class DeleteConversationPolicyRequest
    {
        public Guid ActorId { get; set; }
        public Guid ConversationId { get; set; }
    }
}
