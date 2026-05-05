using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Rules.Conversations;
using ChatApp.Application.Authorization.Queries.Conversation.CreateConversation;

namespace ChatApp.Application.Authorization.Policies.Conversation
{
    public class CreateConversationPolicy : IPolicy<CreateConversationPolicyRequest>
    {
        private readonly CreateConversationAuthorizationQuery _authorizationQuery;
        private readonly CreateConversationRules _rules;
        public CreateConversationPolicy(
            CreateConversationAuthorizationQuery authorizationQuery,
            CreateConversationRules rules
            )
        {
            _authorizationQuery = authorizationQuery;
            _rules = rules;
        }

        public async Task<PolicyResult> EvaluateAsync(CreateConversationPolicyRequest request, CancellationToken cancellationToken = default)
        {
            var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
            var failureReason = _rules.GetFailureReason(data);

            if (failureReason != null)
                return PolicyResult.Deny(failureReason);

            return PolicyResult.Allow();
        }
    
    }
}
