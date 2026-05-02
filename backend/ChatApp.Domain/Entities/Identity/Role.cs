namespace ChatApp.Domain.Entities.Identity
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public bool IsSystemRole { get; set; } = false; // System roles cannot be deleted

        // Many-to-many relationship with Permission
        public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();

        // Many-to-many relationship with User
        public ICollection<UserRole> Users { get; set; } = new List<UserRole>();

        public Role(string name, string? description = null, bool isSystemRole = false)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            IsSystemRole = isSystemRole;
        }

        public Role()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Adds a permission to the role if not already present
        /// </summary>
        public void AddPermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            if (Permissions.Any(rp => rp.PermissionId == permission.Id))
                return; // Permission already assigned

            Permissions.Add(new RolePermission
            {
                RoleId = Id,
                PermissionId = permission.Id,
                Permission = permission
            });
        }

        /// <summary>
        /// Removes a permission from the role
        /// </summary>
        public void RemovePermission(Permission permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission));

            var rolePermission = Permissions.FirstOrDefault(rp => rp.PermissionId == permission.Id);
            if (rolePermission != null)
            {
                Permissions.Remove(rolePermission);
            }
        }

        /// <summary>
        /// Checks if the role has a specific permission
        /// </summary>
        public bool HasPermission(string permissionCodename)
        {
            return Permissions.Any(rp => rp.Permission.Codename == permissionCodename);
        }

        /// <summary>
        /// Gets all permission codenames for this role
        /// </summary>
        public IEnumerable<string> GetPermissionCodenames()
        {
            return Permissions.Select(rp => rp.Permission.Codename);
        }
    }
}
