namespace ChatApp.Domain.Entities.Identity
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;
    }
}
