using Applications.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Applications.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string IdentificationNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null)
                return Result.Failure("Usuario no encontrado");

            // Verificar si el número de identificación ya existe (excluyendo el usuario actual)
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.IdentificationNumber == request.IdentificationNumber && u.Id != request.Id);

            if (existingUser != null)
                return Result.Failure("Ya existe un usuario con este número de identificación");

            user.IdentificationNumber = request.IdentificationNumber;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Phone = request.Phone;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
