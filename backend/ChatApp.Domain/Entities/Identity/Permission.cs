namespace ChatApp.Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; private set; }
        public string Codename { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Many-to-many relationship with Role
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        public Permission(string codename, string name, string? description = null)
        {
            Id = Guid.NewGuid();
            Codename = codename;
            Name = name;
            Description = description;
        }

        public Permission()
        {
            Id = Guid.NewGuid();
        }
    }
}
