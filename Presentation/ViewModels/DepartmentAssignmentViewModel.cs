using Applications.Departments.Commands;
using Applications.Departments.Dtos;
using Applications.Departments.Queries;
using Applications.Users.Dtos;
using Applications.Users.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace Presentation.ViewModels
{
    public partial class DepartmentAssignmentViewModel : ObservableObject
    {
        private readonly IMediator _mediator;

        [ObservableProperty]
        private List<DepartmentDto> _departments = new();

        [ObservableProperty]
        private List<UserDto> _users = new();

        [ObservableProperty]
        private List<UserDepartmentAssignmentDto> _userAssignments = new();

        [ObservableProperty]
        private DepartmentDto? _selectedDepartment;

        [ObservableProperty]
        private UserDto? _selectedUser;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private string _searchText = string.Empty;

        public DepartmentAssignmentViewModel(IMediator mediator)
        {
            _mediator = mediator;
            LoadDepartmentsCommand.Execute(null);
            LoadUsersCommand.Execute(null);
        }

        // Propiedad computada que notifica cambios
        public bool CanAssignUser => SelectedUser != null && SelectedDepartment != null;

        [RelayCommand]
        private async Task LoadDepartments()
        {
            try
            {
                var departments = await _mediator.Send(new GetAllDepartmentsQuery());
                Departments = departments;
                StatusMessage = $"✅ Se cargaron {departments.Count} departamentos";

                // Notificar cambios en propiedades computadas
                OnPropertyChanged(nameof(CanAssignUser));
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al cargar departamentos: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task LoadUsers()
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery());
                Users = users;

                // También cargar las asignaciones actuales
                await LoadUserAssignments();

                StatusMessage = $"✅ Se cargaron {users.Count} usuarios";

                // Notificar cambios en propiedades computadas
                OnPropertyChanged(nameof(CanAssignUser));
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al cargar usuarios: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task LoadUserAssignments()
        {
            try
            {
                // Implementar query para obtener asignaciones actuales
                var assignments = await _mediator.Send(new GetUserAssignmentsQuery());
                UserAssignments = assignments;

                if (assignments.Any())
                {
                    StatusMessage = $"✅ Se cargaron {assignments.Count} asignaciones";
                }
                else
                {
                    StatusMessage = "ℹ️ No hay asignaciones registradas";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al cargar asignaciones: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task AssignUserToDepartment()
        {
            if (SelectedUser == null || SelectedDepartment == null)
            {
                StatusMessage = "❌ Por favor seleccione un usuario y un departamento";
                return;
            }

            try
            {
                var command = new AssignUserToDepartmentCommand
                {
                    UserId = SelectedUser.Id,
                    DepartmentId = SelectedDepartment.Id
                };

                var result = await _mediator.Send(command);
                if (result.Succeeded)
                {
                    StatusMessage = $"✅ Usuario '{SelectedUser.FirstName} {SelectedUser.LastName}' asignado al departamento '{SelectedDepartment.Name}'";

                    // Limpiar selecciones
                    SelectedUser = null;
                    SelectedDepartment = null;

                    // Recargar datos
                    await LoadUserAssignments();
                    await LoadUsers(); // Recargar usuarios para reflejar cambios

                    // Notificar cambios en propiedades computadas
                    OnPropertyChanged(nameof(CanAssignUser));
                }
                else
                {
                    StatusMessage = $"❌ Error: {string.Join(", ", result.Errors)}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al asignar: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task SearchUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadUsers();
                return;
            }

            try
            {
                var allUsers = await _mediator.Send(new GetAllUsersQuery());
                var filteredUsers = allUsers
                    .Where(u => u.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               u.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               u.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               u.IdentificationNumber.Contains(SearchText))
                    .ToList();

                Users = filteredUsers;
                StatusMessage = $"🔍 Se encontraron {filteredUsers.Count} usuarios para '{SearchText}'";

                // Notificar cambios en propiedades computadas
                OnPropertyChanged(nameof(CanAssignUser));
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error en la búsqueda: {ex.Message}";
            }
        }

        [RelayCommand]
        private async Task ClearSearch()
        {
            SearchText = string.Empty;
            await LoadUsers();
        }

        [RelayCommand]
        private async Task RefreshAll()
        {
            await LoadDepartments();
            await LoadUsers();
            StatusMessage = "🔄 Todos los datos han sido actualizados";
        }

        // Métodos parciales para notificar cambios en propiedades computadas
        partial void OnSelectedUserChanged(UserDto? value)
        {
            OnPropertyChanged(nameof(CanAssignUser));
            UpdateStatusMessage();
        }

        partial void OnSelectedDepartmentChanged(DepartmentDto? value)
        {
            OnPropertyChanged(nameof(CanAssignUser));
            UpdateStatusMessage();
        }

        private void UpdateStatusMessage()
        {
            if (SelectedUser != null && SelectedDepartment != null)
            {
                StatusMessage = $"✅ Listo para asignar: {SelectedUser.FirstName} {SelectedUser.LastName} → {SelectedDepartment.Name}";
            }
            else if (SelectedUser != null)
            {
                StatusMessage = "ℹ️ Seleccione un departamento para completar la asignación";
            }
            else if (SelectedDepartment != null)
            {
                StatusMessage = "ℹ️ Seleccione un usuario para completar la asignación";
            }
        }
    }
}
