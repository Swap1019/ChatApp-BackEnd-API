using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Conversations.UrlGroupCreation;
using ChatApp.Application.Authorization.Rules.Conversations;

namespace ChatApp.Application.Authorization.Policies.Conversations.UrlGroupCreation;

public class UrlGroupCreationPolicy : IPolicy<UrlGroupCreationPolicyRequest>
{
    private readonly UrlGroupCreationAuthorizationQuery _authorizationQuery;
    private readonly UrlGroupCreationRules _rules;

    public UrlGroupCreationPolicy(
        UrlGroupCreationAuthorizationQuery authorizationQuery,
        UrlGroupCreationRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(UrlGroupCreationPolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
