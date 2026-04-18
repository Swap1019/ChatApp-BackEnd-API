using ChatApp.Domain.Entities;

namespace ChatApp.Infrastructure.Seeding
{
    public static class PermissionSeeder
    {
        public static List<Permission> GetDefaultPermissions()
        {
            return new List<Permission>
            {
                // User Management Permissions
                new Permission("manage.users.view", "View Users", "View all users and their details"),
                new Permission("manage.users.suspend", "Suspend Users", "Suspend or unsuspend user accounts"),
                new Permission("manage.users.ban", "Ban Users", "Ban users from the platform"),
                new Permission("manage.users.delete", "Delete Users", "Delete user accounts and associated data"),
                new Permission("manage.users.edit", "Edit Users", "Edit user profiles and settings"),

                // Conversation Management Permissions
                new Permission("manage.conversations.view", "View Conversations", "View all conversations"),
                new Permission("manage.conversations.delete", "Delete Conversations", "Delete user conversations"),
                new Permission("manage.conversations.moderate", "Moderate Conversations", "Moderate conversation content"),
                new Permission("manage.conversations.archive", "Archive Conversations", "Archive conversations"),

                // Content Moderation Permissions
                new Permission("moderate.content.view", "View Reported Content", "View flagged or reported content"),
                new Permission("moderate.content.review", "Review Content", "Review and approve/reject flagged content"),
                new Permission("moderate.messages.delete", "Delete Messages", "Delete messages from conversations"),
                new Permission("moderate.posts.approve", "Approve Posts", "Approve or reject user posts"),

                // Role & Permission Management
                new Permission("manage.roles.view", "View Roles", "View all roles and permissions"),
                new Permission("manage.roles.create", "Create Roles", "Create new roles"),
                new Permission("manage.roles.edit", "Edit Roles", "Edit existing roles"),
                new Permission("manage.roles.delete", "Delete Roles", "Delete roles"),
                new Permission("manage.roles.assign", "Assign Roles", "Assign roles to users"),
                new Permission("manage.permissions.edit", "Edit Permissions", "Edit permission definitions"),

                // System Management Permissions
                new Permission("manage.system.view", "View System", "Access system administration panel"),
                new Permission("manage.system.settings", "Manage Settings", "Modify system settings and configuration"),
                new Permission("manage.system.logs", "View Logs", "View system logs and audit trail"),

                // Analytics & Reporting
                new Permission("view.analytics", "View Analytics", "Access analytics and statistics"),
                new Permission("view.reports", "View Reports", "Access user reports and complaint logs"),

                // Security Permissions
                new Permission("manage.security.keys", "Manage API Keys", "Create and revoke API keys"),
                new Permission("manage.security.audit", "Audit Access", "View security audit logs"),
            };
        }

        public static Role GetDefaultAdministratorRole()
        {
            return new Role(
                "Administrator",
                "System administrator with full access to all features",
                isSystemRole: true
            );
        }

        public static Role GetDefaultModeratorRole()
        {
            return new Role(
                "Moderator",
                "Content moderator with permissions to review and moderate user content",
                isSystemRole: true
            );
        }

        public static Role GetDefaultUserRole()
        {
            return new Role(
                "User",
                "Standard user role with basic permissions",
                isSystemRole: true
            );
        }

        /// <summary>
        /// Gets the permission assignments for each default role
        /// </summary>
        public static Dictionary<string, List<string>> GetDefaultRolePermissions()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    "Administrator",
                    new List<string>
                    {
                        // All permissions
                        "manage.users.view",
                        "manage.users.suspend",
                        "manage.users.ban",
                        "manage.users.delete",
                        "manage.users.edit",
                        "manage.conversations.view",
                        "manage.conversations.delete",
                        "manage.conversations.moderate",
                        "manage.conversations.archive",
                        "moderate.content.view",
                        "moderate.content.review",
                        "moderate.messages.delete",
                        "moderate.posts.approve",
                        "manage.roles.view",
                        "manage.roles.create",
                        "manage.roles.edit",
                        "manage.roles.delete",
                        "manage.roles.assign",
                        "manage.permissions.edit",
                        "manage.system.view",
                        "manage.system.settings",
                        "manage.system.logs",
                        "view.analytics",
                        "view.reports",
                        "manage.security.keys",
                        "manage.security.audit"
                    }
                },
                {
                    "Moderator",
                    new List<string>
                    {
                        "manage.conversations.view",
                        "manage.conversations.moderate",
                        "moderate.content.view",
                        "moderate.content.review",
                        "moderate.messages.delete",
                        "moderate.posts.approve",
                        "manage.roles.view",
                        "view.reports",
                    }
                },
                {
                    "User",
                    new List<string>
                    {
                        // Users get no special permissions beyond their standard conversation access
                    }
                }
            };
        }
    }
}
