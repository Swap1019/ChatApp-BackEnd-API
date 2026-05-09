using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Messages.PinMessage;
using ChatApp.Application.Authorization.Rules.Messages;

namespace ChatApp.Application.Authorization.Policies.Messages.PinMessage;

public class PinMessagePolicy : IPolicy<PinMessagePolicyRequest>
    {
    private readonly PinMessageAuthorizationQuery _authorizationQuery;
    private readonly PinMessageRules _rules;

    public PinMessagePolicy(
        PinMessageAuthorizationQuery authorizationQuery,
        PinMessageRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(PinMessagePolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
