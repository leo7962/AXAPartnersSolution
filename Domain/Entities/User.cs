using Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User : Entity
    {
        public string IdentificationNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
    }
}
