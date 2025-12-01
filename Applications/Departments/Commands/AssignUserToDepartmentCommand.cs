using Applications.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace Applications.Departments.Commands
{
    public class AssignUserToDepartmentCommand : IRequest<Result>
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
    }
    public class AssignUserToDepartmentCommandHandler : IRequestHandler<AssignUserToDepartmentCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public AssignUserToDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AssignUserToDepartmentCommand request, CancellationToken cancellationToken)
        {
            // Verificar si el usuario existe
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return Result.Failure("Usuario no encontrado");

            // Verificar si el departamento existe
            var department = await _context.Departments.FindAsync(request.DepartmentId);
            if (department == null)
                return Result.Failure("Departamento no encontrado");

            // Verificar si ya está asignado
            var existingAssignment = await _context.UserDepartments
                .FirstOrDefaultAsync(ud => ud.UserId == request.UserId && ud.DepartmentId == request.DepartmentId);

            if (existingAssignment != null)
                return Result.Failure("El usuario ya está asignado a este departamento");

            // Crear nueva asignación
            var userDepartment = new Domain.Entities.UserDepartment
            {
                UserId = request.UserId,
                DepartmentId = request.DepartmentId,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserDepartments.Add(userDepartment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
