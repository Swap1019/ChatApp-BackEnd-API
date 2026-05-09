using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Conversations.JoinConversation;
using ChatApp.Application.Authorization.Rules.Conversations;

namespace ChatApp.Application.Authorization.Policies.Conversations.JoinConversation;

public class JoinConversationPolicy : IPolicy<JoinConversationPolicyRequest>
    {
    private readonly JoinConversationAuthorizationQuery _authorizationQuery;
    private readonly JoinConversationRules _rules;

    public JoinConversationPolicy(
        JoinConversationAuthorizationQuery authorizationQuery,
        JoinConversationRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(JoinConversationPolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
