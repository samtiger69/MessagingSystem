namespace Domain.Entities
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        public bool IsGlobal { get; set; }
        public int UserId { get; set; }

        public PermissionType PermissionType { get; set; }

        public virtual User User { get; set; }
    }
}
