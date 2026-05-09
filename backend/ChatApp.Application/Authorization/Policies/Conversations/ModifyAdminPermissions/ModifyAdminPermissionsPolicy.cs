using ChatApp.Application.Authorization.Common;
using ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions;
using ChatApp.Application.Authorization.Rules.Conversations;

namespace ChatApp.Application.Authorization.Policies.Conversations.ModifyAdminPermissions;

public class ModifyAdminPermissionsPolicy : IPolicy<ModifyAdminPermissionsPolicyRequest>
{
    private readonly ModifyAdminPermissionsAuthorizationQuery _authorizationQuery;
    private readonly ModifyAdminPermissionsRules _rules;

    public ModifyAdminPermissionsPolicy(
        ModifyAdminPermissionsAuthorizationQuery authorizationQuery,
        ModifyAdminPermissionsRules rules)
    {
        _authorizationQuery = authorizationQuery;
        _rules = rules;
    }

    public async Task<PolicyResult> EvaluateAsync(ModifyAdminPermissionsPolicyRequest request, CancellationToken cancellationToken = default)
    {
        var data = await _authorizationQuery.ExecuteAsync(request, cancellationToken);
        var failureReason = _rules.GetFailureReason(data);

        if (failureReason != null)
            return PolicyResult.Deny(failureReason);

        return PolicyResult.Allow();
    }
}
