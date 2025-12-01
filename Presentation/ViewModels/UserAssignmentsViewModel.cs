using Applications.Departments.Commands;
using Applications.Departments.Dtos;
using Applications.Departments.Queries;
using Applications.Users.Dtos;
using Applications.Users.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows;

namespace Presentation.ViewModels
{
    public partial class UserAssignmentsViewModel : ObservableObject
    {
        private readonly IMediator _mediator;

        [ObservableProperty]
        private ObservableCollection<UserDepartmentAssignmentDto> _userAssignments = new();

        [ObservableProperty]
        private ObservableCollection<UserDto> _allUsers = new();

        [ObservableProperty]
        private ObservableCollection<DepartmentDto> _allDepartments = new();

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private UserDepartmentAssignmentDto? _selectedAssignment;

        [ObservableProperty]
        private bool _isBusy = false;

        public UserAssignmentsViewModel(IMediator mediator)
        {
            _mediator = mediator;
            // No ejecutar LoadData automáticamente, dejarlo para el usuario
            StatusMessage = "👈 Haga clic en 'Cargar Datos' para comenzar";
        }

        [RelayCommand(CanExecute = nameof(CanLoadData))]
        private async Task LoadData()
        {
            if (IsBusy) return;

            IsBusy = true;
            StatusMessage = "🔄 Cargando datos...";

            try
            {
                // Ejecutar las consultas de forma secuencial para evitar problemas de concurrencia
                var assignments = await _mediator.Send(new GetUserAssignmentsQuery());
                var users = await _mediator.Send(new GetAllUsersQuery());
                var departments = await _mediator.Send(new GetAllDepartmentsQuery());

                // Actualizar las colecciones en el hilo de la UI
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    UserAssignments = new ObservableCollection<UserDepartmentAssignmentDto>(assignments);
                    AllUsers = new ObservableCollection<UserDto>(users);
                    AllDepartments = new ObservableCollection<DepartmentDto>(departments);
                });

                StatusMessage = $"✅ Datos cargados: {assignments.Count} asignaciones, {users.Count} usuarios, {departments.Count} departamentos";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al cargar datos: {ex.Message}";
                if (ex.InnerException != null)
                {
                    StatusMessage += $"\nDetalle: {ex.InnerException.Message}";
                }
            }
            finally
            {
                IsBusy = false;
                // Notificar a los comandos que pueden haber cambiado su estado de ejecución
                LoadDataCommand.NotifyCanExecuteChanged();
                SearchAssignmentsCommand.NotifyCanExecuteChanged();
                RemoveAssignmentCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand(CanExecute = nameof(CanSearchAssignments))]
        private async Task SearchAssignments()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadData();
                return;
            }

            IsBusy = true;
            StatusMessage = $"🔍 Buscando '{SearchText}'...";

            try
            {
                var allAssignments = await _mediator.Send(new GetUserAssignmentsQuery());
                var filteredAssignments = allAssignments
                    .Where(a => a.UserName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               a.DepartmentName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    UserAssignments = new ObservableCollection<UserDepartmentAssignmentDto>(filteredAssignments);
                });

                StatusMessage = $"✅ Se encontraron {filteredAssignments.Count} asignaciones para '{SearchText}'";
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error en la búsqueda: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                LoadDataCommand.NotifyCanExecuteChanged();
                SearchAssignmentsCommand.NotifyCanExecuteChanged();
                RemoveAssignmentCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand(CanExecute = nameof(CanRemoveAssignment))]
        private async Task RemoveAssignment()
        {
            if (SelectedAssignment == null || IsBusy)
            {
                StatusMessage = "❌ Por favor seleccione una asignación para eliminar";
                return;
            }

            IsBusy = true;
            StatusMessage = "🗑️ Eliminando asignación...";

            try
            {
                var command = new RemoveUserAssignmentCommand
                {
                    UserId = SelectedAssignment.UserId,
                    DepartmentId = SelectedAssignment.DepartmentId
                };

                var result = await _mediator.Send(command);
                if (result.Succeeded)
                {
                    StatusMessage = $"✅ Asignación eliminada correctamente";
                    await LoadData();
                }
                else
                {
                    StatusMessage = $"❌ Error al eliminar: {string.Join(", ", result.Errors)}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al eliminar asignación: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                LoadDataCommand.NotifyCanExecuteChanged();
                SearchAssignmentsCommand.NotifyCanExecuteChanged();
                RemoveAssignmentCommand.NotifyCanExecuteChanged();
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
            // No ejecutar LoadData aquí, dejar que el usuario lo haga manualmente
            StatusMessage = "ℹ️ Busqueda limpiada. Haga clic en 'Cargar Datos' para ver todas las asignaciones.";
        }

        // Propiedades para controlar la ejecución de comandos
        private bool CanLoadData => !IsBusy;
        private bool CanRemoveAssignment => SelectedAssignment != null && !IsBusy;
        private bool CanSearchAssignments => !IsBusy;

        // Método parcial para notificar cambios cuando IsBusy cambia
        partial void OnIsBusyChanged(bool value)
        {
            LoadDataCommand.NotifyCanExecuteChanged();
            SearchAssignmentsCommand.NotifyCanExecuteChanged();
            RemoveAssignmentCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedAssignmentChanged(UserDepartmentAssignmentDto? value)
        {
            RemoveAssignmentCommand.NotifyCanExecuteChanged();
        }
    }
}
