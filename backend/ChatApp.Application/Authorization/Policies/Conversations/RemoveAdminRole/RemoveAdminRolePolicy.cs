using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole;
using ChatApp.Application.Authorization.Rules.Conversations;

namespace ChatApp.Application.Authorization.Policies.Conversation
{
    public class RemoveAdminRolePolicy : IPolicy<RemoveAdminRolePolicyRequest>
    {
        private readonly RemoveAdminRoleAuthorizationQuery _authorizationQuery;
        private readonly RemoveAdminRoleRules _rules;

        public RemoveAdminRolePolicy(
            RemoveAdminRoleAuthorizationQuery authorizationQuery,
            RemoveAdminRoleRules rules)
        {
            _authorizationQuery = authorizationQuery;
            _rules = rules;
        }

        public async Task<PolicyResult> EvaluateAsync(RemoveAdminRolePolicyRequest request, CancellationToken cancellationToken = default)
        {
            var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
            var failureReason = _rules.GetFailureReason(data);

            if (failureReason != null)
                return PolicyResult.Deny(failureReason);

            return PolicyResult.Allow();
        }
    }
}
