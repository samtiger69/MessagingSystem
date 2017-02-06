using System.Collections.Generic;

namespace Domain.Entities
{
    public class Organization : BaseEntity
    {
        public int ParentOrganizationId { get; set; }

        public bool IsDeleted { get; set; }
        public virtual Organization ParentOrganization { get; set; }
        public virtual IList<Organization> Children { get; set; }

        public override bool Equals(object y)
        {
            return this.Id == ((Organization)(y)).Id;
        }
        public override int GetHashCode()
        {
            return this.Id;
        }

    }
}
