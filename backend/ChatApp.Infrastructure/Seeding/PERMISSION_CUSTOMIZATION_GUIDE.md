# Role Permission Customization Guide

## Overview

The permission system is now fully customizable through a JSON configuration file, allowing different admins to have different permission sets without code changes.

## How It Works

### 1. **Configuration File**: `role-permissions-config.json`

The configuration file defines:
- **Permissions**: Available actions (e.g., `manage.users.suspend`)
- **Roles**: Role definitions with their assigned permissions
- **Assignments**: Which permissions each role has

### 2. **Key Components**

#### Permission Management Methods (Role Entity)
- `AddPermission(permission)` - Add a single permission to a role
- `RemovePermission(permission)` - Remove a permission from a role
- `HasPermission(codename)` - Check if role has a permission
- `GetPermissionCodenames()` - Get all permission codenames

#### RolePermissionService
- `AssignPermissionsToRoleAsync()` - Assign permissions by codename
- `ClearRolePermissionsAsync()` - Remove all permissions
- `UpdateRolePermissionsAsync()` - Replace all permissions with new ones

#### ConfigBasedPermissionSeeder
- `LoadConfigAsync()` - Load configuration from JSON file
- `SeedFromConfigAsync()` - Apply configuration to database

## Customizing Permissions

### Edit the JSON Configuration

```json
{
  "roles": [
    {
      "name": "Administrator",
      "permissions": [
        "manage.users.view",
        "manage.users.suspend",
        "manage.conversations.view"
        // Remove: "manage.users.delete" to prevent deletion
        // Add custom permission codenames here
      ]
    }
  ]
}
```

### Important: Permission Scenarios

#### Scenario 1: Multiple Admins with Different Powers
```json
{
  "roles": [
    {
      "name": "Senior Administrator",
      "permissions": [
        // Full access
        "manage.users.view",
        "manage.users.suspend",
        "manage.users.ban",
        "manage.users.delete",
        // ... all permissions
      ]
    },
    {
      "name": "Junior Administrator",
      "permissions": [
        "manage.users.view",
        "manage.conversations.view",
        "moderate.content.view"
        // Limited to viewing only, no deletion/banning
      ]
    }
  ]
}
```

#### Scenario 2: Environment-Specific Permissions
```json
{
  "roles": [
    {
      "name": "Administrator",
      "permissions": [
        // Production: Limited permissions
        "manage.users.view",
        "manage.conversations.moderate",
        "view.reports"
      ]
    }
  ]
}
```

#### Scenario 3: Strict Content Moderators
```json
{
  "roles": [
    {
      "name": "Moderator",
      "permissions": [
        "moderate.content.view",
        "moderate.content.review",
        "moderate.messages.delete",
        "view.reports",
        // No role management or system access
      ]
    }
  ]
}
```

## Usage in Code

### Seeding from Configuration
```csharp
// Load configuration
var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
    "Seeding/role-permissions-config.json");
var config = await ConfigBasedPermissionSeeder.LoadConfigAsync(configPath);

// Seed roles and permissions
await ConfigBasedPermissionSeeder.SeedFromConfigAsync(dbContext, config);
```

### Programmatically Updating Permissions at Runtime
```csharp
var service = new RolePermissionService();
var role = await dbContext.Roles.Include(r => r.Permissions).FirstAsync(r => r.Name == "Administrator");
var permissions = await dbContext.Permissions.ToListAsync();

// Update role permissions
var newPermissions = new[] { 
    "manage.users.view", 
    "manage.conversations.view" 
};

await service.UpdateRolePermissionsAsync(role, newPermissions, permissions);
await dbContext.SaveChangesAsync();
```

### Checking Permissions
```csharp
var role = await dbContext.Roles.Include(r => r.Permissions)
    .FirstAsync(r => r.Name == "Administrator");

if (role.HasPermission("manage.users.delete"))
{
    // Allow user deletion
}

var allPermissions = role.GetPermissionCodenames();
```

## Adding New Permissions

To add new permissions:

1. Add to `role-permissions-config.json`:
```json
{
  "permissions": [
    {
      "codename": "custom.feature.execute",
      "name": "Execute Custom Feature",
      "description": "Permission to execute new custom feature"
    }
  ]
}
```

2. Assign to roles in the same file:
```json
{
  "roles": [
    {
      "name": "Administrator",
      "permissions": ["custom.feature.execute"]
    }
  ]
}
```

3. Reseed the database

## API Endoints (To Implement)

These would use the `RolePermissionService`:

- `PUT /api/admin/roles/{roleId}/permissions` - Update role permissions
- `GET /api/admin/roles/{roleId}/permissions` - Get role permissions
- `POST /api/admin/roles/{roleId}/permissions/{permissionId}` - Add permission
- `DELETE /api/admin/roles/{roleId}/permissions/{permissionId}` - Remove permission

## Best Practices

1. ✅ Keep system roles (`Administrator`, `Moderator`, `User`) for consistency
2. ✅ Use semantic permission codenames: `resource.action.scope`
3. ✅ Document custom permissions in the JSON file
4. ✅ Test permission checks before deployment
5. ✅ Review role permissions regularly
6. ✅ Avoid giving deletion permissions unless necessary

## Reverting/Resetting Permissions

To reset all permissions to defaults, simply re-run the seeder with the original configuration file.
