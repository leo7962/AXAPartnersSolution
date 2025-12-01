using Applications.Common.Interfaces;
using Applications.Departments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Departments.Queries
{
    public class GetUserAssignmentsQuery : IRequest<List<UserDepartmentAssignmentDto>>
    {
    }
    public class GetUserAssignmentsQueryHandler : IRequestHandler<GetUserAssignmentsQuery, List<UserDepartmentAssignmentDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUserAssignmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDepartmentAssignmentDto>> Handle(GetUserAssignmentsQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _context.UserDepartments
                .Where(ud => ud.User.IsActive)
                .Select(ud => new UserDepartmentAssignmentDto
                {
                    UserId = ud.UserId,
                    UserName = $"{ud.User.FirstName} {ud.User.LastName}",
                    DepartmentId = ud.DepartmentId,
                    DepartmentName = ud.Department.Name,
                    AssignedAt = ud.AssignedAt
                })
                .ToListAsync(cancellationToken);

            return assignments;
        }
    }
}
