using ChatApp.Domain.Entities;

namespace ChatApp.Infrastructure.Services
{
    public interface IRolePermissionService
    {
        Task AssignPermissionsToRoleAsync(Role role, IEnumerable<string> permissionCodenames, IEnumerable<Permission> availablePermissions);
        Task ClearRolePermissionsAsync(Role role);
        Task UpdateRolePermissionsAsync(Role role, IEnumerable<string> permissionCodenames, IEnumerable<Permission> availablePermissions);
    }

    public class RolePermissionService : IRolePermissionService
    {
        /// <summary>
        /// Assigns permissions to a role based on permission codenames
        /// </summary>
        public Task AssignPermissionsToRoleAsync(Role role, IEnumerable<string> permissionCodenames, IEnumerable<Permission> availablePermissions)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var permissionCodeList = permissionCodenames?.ToList() ?? new List<string>();
            var permissionsList = availablePermissions?.ToList() ?? new List<Permission>();

            foreach (var codename in permissionCodeList)
            {
                var permission = permissionsList.FirstOrDefault(p => p.Codename == codename);
                if (permission != null && !role.HasPermission(codename))
                {
                    role.AddPermission(permission);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears all permissions from a role
        /// </summary>
        public Task ClearRolePermissionsAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Permissions.Clear();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates role permissions by clearing existing and assigning new ones
        /// </summary>
        public async Task UpdateRolePermissionsAsync(Role role, IEnumerable<string> permissionCodenames, IEnumerable<Permission> availablePermissions)
        {
            await ClearRolePermissionsAsync(role);
            await AssignPermissionsToRoleAsync(role, permissionCodenames, availablePermissions);
        }
    }
}
