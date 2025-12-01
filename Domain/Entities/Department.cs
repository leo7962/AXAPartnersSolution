using Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Department : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    }
}
