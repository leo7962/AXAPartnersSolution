using Applications.Common.Interfaces;
using MediatR;
using Shared.Common;

namespace Applications.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public DeleteUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null)
                return Result.Failure("Usuario no encontrado");

            // En lugar de eliminar físicamente, marcamos como inactivo (soft delete)
            user.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
