using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Applications.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Department> Departments { get; }
        DbSet<UserDepartment> UserDepartments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
