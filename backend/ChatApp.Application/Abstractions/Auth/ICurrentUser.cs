namespace ChatApp.Application.Abstractions.Auth;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    string? Username { get; }

    IReadOnlyCollection<string> RoleNames { get; }

    IReadOnlyCollection<string> PermissionCodenames { get; }

    bool HasRole(string roleName);

    bool HasPermission(string permissionCodename);
}
