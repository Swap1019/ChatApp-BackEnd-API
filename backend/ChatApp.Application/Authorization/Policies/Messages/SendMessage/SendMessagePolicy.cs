using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Messages.SendMessage;
using ChatApp.Application.Authorization.Rules.Messages;

namespace ChatApp.Application.Authorization.Policies.Messages.SendMessage;

public class SendMessagePolicy : IPolicy<SendMessagePolicyRequest>
{
    private readonly SendMessageAuthorizationQuery _authorizationQuery;
    private readonly SendMessageRules _rules;

    public SendMessagePolicy(
        SendMessageAuthorizationQuery authorizationQuery,
        SendMessageRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(SendMessagePolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
