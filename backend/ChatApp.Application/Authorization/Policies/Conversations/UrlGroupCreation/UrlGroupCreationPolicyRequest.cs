namespace ChatApp.Application.Authorization.Policies.Conversations.UrlGroupCreation;

public class UrlGroupCreationPolicyRequest
{
    public Guid ActorId { get; set; }
    public Guid ConversationId { get; set; }
}