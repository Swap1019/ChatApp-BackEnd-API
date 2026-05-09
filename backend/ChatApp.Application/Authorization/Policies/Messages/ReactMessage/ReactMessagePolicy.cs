using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Messages.ReactMessage;
using ChatApp.Application.Authorization.Rules.Messages;

namespace ChatApp.Application.Authorization.Policies.Messages.ReactMessage;

public class ReactMessagePolicy : IPolicy<ReactMessagePolicyRequest>
    {
    private readonly ReactMessageAuthorizationQuery _authorizationQuery;
    private readonly ReactMessageRules _rules;

    public ReactMessagePolicy(
        ReactMessageAuthorizationQuery authorizationQuery,
        ReactMessageRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(ReactMessagePolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
