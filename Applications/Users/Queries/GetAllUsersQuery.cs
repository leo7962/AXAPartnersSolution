using Applications.Common.Interfaces;
using Applications.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Users.Queries
{
    public class GetAllUsersQuery : IRequest<List<UserDto>>
    {
    }
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    IdentificationNumber = u.IdentificationNumber,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return users;
        }
    }
}
