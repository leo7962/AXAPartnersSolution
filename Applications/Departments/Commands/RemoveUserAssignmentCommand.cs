using Applications.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Applications.Departments.Commands
{
    public class RemoveUserAssignmentCommand : IRequest<Result>
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
    }

    public class RemoveUserAssignmentCommandHandler : IRequestHandler<RemoveUserAssignmentCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public RemoveUserAssignmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(RemoveUserAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await _context.UserDepartments
                .FirstOrDefaultAsync(ud => ud.UserId == request.UserId && ud.DepartmentId == request.DepartmentId);

            if (assignment == null)
                return Result.Failure("Asignación no encontrada");

            _context.UserDepartments.Remove(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
