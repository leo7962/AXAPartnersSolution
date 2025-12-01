using Shared.Common;

namespace Domain.Entities
{
    public class UserDepartment : Entity
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
    }
}
