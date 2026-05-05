using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Rules.Conversations;
using ChatApp.Application.Authorization.Queries.Conversation.DeleteConversation;

namespace ChatApp.Application.Authorization.Policies.Conversation
{
    public class DeleteConversationPolicy : IPolicy<DeleteConversationPolicyRequest>
    {
        private readonly DeleteConversationAuthorizationQuery _authorizationQuery;
        private readonly DeleteConversationRules _rules;

        public DeleteConversationPolicy(DeleteConversationAuthorizationQuery authorizationQuery, DeleteConversationRules rules)
        {
            _authorizationQuery = authorizationQuery;
            _rules = rules;
        }

        public async Task<PolicyResult> EvaluateAsync(DeleteConversationPolicyRequest request, CancellationToken cancellationToken = default)
        {
            var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
            var failureReason = _rules.GetFailureReason(data);

            if (failureReason != null)
                return PolicyResult.Deny(failureReason);

            return PolicyResult.Allow();
        }
    
    }
}
