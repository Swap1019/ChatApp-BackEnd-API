using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Messages.DeleteMessage;
using ChatApp.Application.Authorization.Rules.Messages;

namespace ChatApp.Application.Authorization.Policies.Messages.DeleteMessage;

public class DeleteMessagePolicy : IPolicy<DeleteMessagePolicyRequest>
    {
    private readonly DeleteMessageAuthorizationQuery _authorizationQuery;
    private readonly DeleteMessageRules _rules;

    public DeleteMessagePolicy(
        DeleteMessageAuthorizationQuery authorizationQuery,
        DeleteMessageRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(DeleteMessagePolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
