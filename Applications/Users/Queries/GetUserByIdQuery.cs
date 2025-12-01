using Applications.Common.Interfaces;
using Applications.Users.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto?>
    {
        public int Id { get; set; }
    }
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetUserByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(u => u.Id == request.Id && u.IsActive)
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
                .FirstOrDefaultAsync(cancellationToken);

            return user;
        }
    }
}
