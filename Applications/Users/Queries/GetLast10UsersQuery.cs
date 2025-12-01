using Applications.Common.Interfaces;
using Applications.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Users.Queries
{
    public class GetLast10UsersQuery : IRequest<List<UserDto>>
    {

    }

    public class GetLast10UsersQueryHandler : IRequestHandler<GetLast10UsersQuery, List<UserDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetLast10UsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> Handle(GetLast10UsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
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