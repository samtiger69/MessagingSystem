using System.Collections.Generic;

namespace Domain.Entities
{
    public class UserType : BaseEntity
    {
        public virtual IList<User> Users { get; set; }
    }
}
