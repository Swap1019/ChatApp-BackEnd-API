using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Conversations.BanUser;
using ChatApp.Application.Authorization.Rules.Conversations;

namespace ChatApp.Application.Authorization.Policies.Conversations.BanUser;

public class BanUserPolicy : IPolicy<BanUserPolicyRequest>
{
    private readonly BanUserAuthorizationQuery _authorizationQuery;
    private readonly BanUserRules _rules;

    public BanUserPolicy(
        BanUserAuthorizationQuery authorizationQuery,
        BanUserRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(BanUserPolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
