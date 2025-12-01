using Applications.Common.Interfaces;
using Applications.Departments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Departments.Queries
{
    public class GetAllDepartmentsQuery : IRequest<List<DepartmentDto>>
    {
    }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDepartmentsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    IsActive = d.IsActive
                })
                .ToListAsync(cancellationToken);

            return departments;
        }
    }
}
