using System;
using System.Collections.Generic;
using System.Text;

namespace Applications.Departments.Dtos
{
    public class UserDepartmentAssignmentDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
