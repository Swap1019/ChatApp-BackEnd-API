namespace ChatApp.Application.Authorization.Policies.Conversation 
{
    public class RemoveAdminRolePolicyRequest
    {
        public Guid ActorId { get; set; }
        public Guid TargetUserId { get; set; }
        public Guid ConversationId { get; set; }
    }
}
