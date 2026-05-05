namespace ChatApp.Application.Authorization.Policies.Conversation 
{
    public class CreateConversationPolicyRequest
    {
        public Guid ActorId { get; set; }
        public Guid? TargetUserId { get; set; }
    }
}
