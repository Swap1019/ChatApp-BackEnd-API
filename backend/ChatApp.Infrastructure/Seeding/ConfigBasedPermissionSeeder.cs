using ChatApp.Domain.Entities.Identity;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ChatApp.Infrastructure.Seeding
{
    public class RolePermissionConfigModel
    {
        public List<PermissionModel> Permissions { get; set; } = new();
        public List<RoleModel> Roles { get; set; } = new();
    }

    public class PermissionModel
    {
        public string Codename { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class RoleModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

    public static class ConfigBasedPermissionSeeder
    {
        /// <summary>
        /// Loads role and permission configuration from a JSON file
        /// </summary>
        public static async Task<RolePermissionConfigModel> LoadConfigAsync(string configPath)
        {
            if (!File.Exists(configPath))
                throw new FileNotFoundException($"Permission configuration file not found: {configPath}");

            var json = await File.ReadAllTextAsync(configPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var config = JsonSerializer.Deserialize<RolePermissionConfigModel>(json, options);

            if (config == null)
                throw new InvalidOperationException("Failed to deserialize permission configuration");

            return config;
        }

        /// <summary>
        /// Seeds permissions and roles from configuration
        /// </summary>
        public static async Task SeedFromConfigAsync(
            AppDbContext context,
            RolePermissionConfigModel config)
        {
            // Create or update permissions
            var permissionsDict = new Dictionary<string, Permission>();
            foreach (var permModel in config.Permissions)
            {
                var existing = await context.Permissions.FirstOrDefaultAsync(
                    p => p.Codename == permModel.Codename);

                if (existing == null)
                {
                    var permission = new Permission
                    {
                        Codename = permModel.Codename,
                        Name = permModel.Name,
                        Description = permModel.Description
                    };
                    context.Permissions.Add(permission);
                    permissionsDict[permModel.Codename] = permission;
                }
                else
                {
                    permissionsDict[permModel.Codename] = existing;
                }
            }

            await context.SaveChangesAsync();

            // Create or update roles
            var roleService = new RolePermissionService();
            foreach (var roleModel in config.Roles)
            {
                var existing = await context.Roles.Include(r => r.Permissions)
                    .FirstOrDefaultAsync(r => r.Name == roleModel.Name);

                Role role;
                if (existing == null)
                {
                    role = new Role
                    {
                        Name = roleModel.Name,
                        Description = roleModel.Description,
                        IsSystemRole = roleModel.IsSystemRole
                    };
                    context.Roles.Add(role);
                }
                else
                {
                    role = existing;
                    role.Description = roleModel.Description;
                    role.IsSystemRole = roleModel.IsSystemRole;
                }

                // Assign permissions based on config
                var permissions = roleModel.Permissions
                    .Where(code => permissionsDict.ContainsKey(code))
                    .Select(code => permissionsDict[code])
                    .ToList();

                await roleService.UpdateRolePermissionsAsync(role, 
                    roleModel.Permissions, 
                    permissions);

                context.Roles.Update(role);
            }

            await context.SaveChangesAsync();
        }
    }
}
