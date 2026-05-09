using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Rules.Conversations;
using ChatApp.Application.Authorization.Queries.Conversations.AddUser;

namespace ChatApp.Application.Authorization.Policies.Conversations.AddUser;

public class AddUserPolicy : IPolicy<AddUserPolicyRequest>
{
    private readonly AddUserAuthorizationQuery _authorizationQuery;
    private readonly AddUserRules _rules;

    public AddUserPolicy(
        AddUserAuthorizationQuery authorizationQuery,
        AddUserRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(AddUserPolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
