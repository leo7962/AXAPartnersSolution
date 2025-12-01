using Applications.Users.Commands;
using Applications.Users.Dtos;
using Applications.Users.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace Presentation.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly IMediator _mediator;

        [ObservableProperty]
        private List<UserDto> _users = new();

        [ObservableProperty]
        private UserDto? _selectedUser;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        // Propiedades para el formulario de usuario con validación
        [ObservableProperty]
        private string _identificationNumber = string.Empty;

        [ObservableProperty]
        private string _firstName = string.Empty;

        [ObservableProperty]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _phone = string.Empty;

        [ObservableProperty]
        private bool _isEditing = false;

        // Propiedades para mensajes de error específicos
        [ObservableProperty]
        private string _identificationNumberError = string.Empty;

        [ObservableProperty]
        private string _firstNameError = string.Empty;

        [ObservableProperty]
        private string _lastNameError = string.Empty;

        [ObservableProperty]
        private string _emailError = string.Empty;

        [ObservableProperty]
        private string _phoneError = string.Empty;

        // Expresión regular más flexible para validar email
        private readonly Regex _emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UserManagementViewModel(IMediator mediator)
        {
            _mediator = mediator;
            LoadUsersCommand.Execute(null);

            // Suscribirse a los cambios de propiedades para validación en tiempo real
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(IdentificationNumber) ||
                    e.PropertyName == nameof(FirstName) ||
                    e.PropertyName == nameof(LastName) ||
                    e.PropertyName == nameof(Email) ||
                    e.PropertyName == nameof(Phone))
                {
                    ValidateForm();
                    OnPropertyChanged(nameof(CanSaveUser));
                    OnPropertyChanged(nameof(IsFormValid));
                    SaveUserCommand.NotifyCanExecuteChanged();
                }
            };
        }

        // Propiedades computadas que notifican cambios
        public bool CanEditUser => SelectedUser != null;
        public bool CanDeleteUser => SelectedUser != null;
        public string SaveButtonText => IsEditing ? "Actualizar Usuario" : "Crear Usuario";

        // Propiedad para habilitar/deshabilitar el botón Guardar
        public bool CanSaveUser => IsFormValid;
        public bool IsFormValid => string.IsNullOrWhiteSpace(IdentificationNumberError) &&
                                 string.IsNullOrWhiteSpace(FirstNameError) &&
                                 string.IsNullOrWhiteSpace(LastNameError) &&
                                 string.IsNullOrWhiteSpace(EmailError) &&
                                 string.IsNullOrWhiteSpace(PhoneError) &&
                                 !string.IsNullOrWhiteSpace(IdentificationNumber) &&
                                 !string.IsNullOrWhiteSpace(FirstName) &&
                                 !string.IsNullOrWhiteSpace(LastName) &&
                                 !string.IsNullOrWhiteSpace(Email) &&
                                 !string.IsNullOrWhiteSpace(Phone);

        [RelayCommand]
        private async Task LoadUsers()
        {
            try
            {
                var users = await _mediator.Send(new GetLast10UsersQuery());
                Users = users;
                StatusMessage = $"✅ Se cargaron {users.Count} usuarios";

                OnPropertyChanged(nameof(CanEditUser));
                OnPropertyChanged(nameof(CanDeleteUser));
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error al cargar usuarios: {ex.Message}";
            }
        }

        [RelayCommand]
        private void CreateNewUser()
        {
            IsEditing = false;
            ClearForm();
            StatusMessage = "📝 Creando nuevo usuario... Complete todos los campos requeridos.";
        }

        [RelayCommand]
        private void EditUser()
        {
            if (SelectedUser == null)
            {
                StatusMessage = "❌ Por favor seleccione un usuario para editar";
                return;
            }

            IsEditing = true;
            LoadUserData(SelectedUser);
            StatusMessage = $"✏️ Editando usuario: {SelectedUser.FirstName} {SelectedUser.LastName}";
            OnPropertyChanged(nameof(SaveButtonText));
        }

        [RelayCommand(CanExecute = nameof(CanSaveUser))]
        private async Task SaveUser()
        {
            if (!IsFormValid)
            {
                StatusMessage = "❌ Por favor complete todos los campos correctamente antes de guardar";
                return;
            }

            try
            {
                if (IsEditing && SelectedUser != null)
                {
                    // Actualizar usuario existente
                    var command = new UpdateUserCommand
                    {
                        Id = SelectedUser.Id,
                        IdentificationNumber = IdentificationNumber.Trim(),
                        FirstName = FirstName.Trim(),
                        LastName = LastName.Trim(),
                        Email = Email.Trim(),
                        Phone = Phone.Trim()
                    };

                    var result = await _mediator.Send(command);
                    if (result.Succeeded)
                    {
                        StatusMessage = "✅ Usuario actualizado correctamente";
                        ClearForm();
                        await LoadUsers();
                    }
                    else
                    {
                        StatusMessage = $"❌ Error al actualizar: {string.Join(", ", result.Errors)}";
                    }
                }
                else
                {
                    // Crear nuevo usuario
                    var command = new CreateUserCommand
                    {
                        IdentificationNumber = IdentificationNumber.Trim(),
                        FirstName = FirstName.Trim(),
                        LastName = LastName.Trim(),
                        Email = Email.Trim(),
                        Phone = Phone.Trim()
                    };

                    var result = await _mediator.Send(command);
                    if (result.Succeeded)
                    {
                        StatusMessage = "✅ Usuario creado correctamente";
                        ClearForm();
                        await LoadUsers();
                    }
                    else
                    {
                        StatusMessage = $"❌ Error al crear: {string.Join(", ", result.Errors)}";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            ClearForm();
            StatusMessage = "ℹ️ Operación cancelada";
        }

        [RelayCommand]
        private async Task DeleteUser()
        {
            if (SelectedUser == null)
            {
                StatusMessage = "❌ Por favor seleccione un usuario para eliminar";
                return;
            }

            var result = MessageBox.Show(
                $"¿Está seguro de que desea eliminar al usuario {SelectedUser.FirstName} {SelectedUser.LastName}?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var deleteCommand = new DeleteUserCommand { Id = SelectedUser.Id };
                    var deleteResult = await _mediator.Send(deleteCommand);

                    if (deleteResult.Succeeded)
                    {
                        StatusMessage = "✅ Usuario eliminado correctamente";
                        await LoadUsers();
                    }
                    else
                    {
                        StatusMessage = $"❌ Error al eliminar: {string.Join(", ", deleteResult.Errors)}";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"❌ Error al eliminar: {ex.Message}";
                }
            }
        }

        // Métodos parciales que se ejecutan cuando las propiedades cambian
        partial void OnSelectedUserChanged(UserDto? value)
        {
            OnPropertyChanged(nameof(CanEditUser));
            OnPropertyChanged(nameof(CanDeleteUser));
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(SaveButtonText));
        }

        // Validación en tiempo real
        private void ValidateForm()
        {
            // Validar Identificación (solo requerida)
            IdentificationNumberError = string.IsNullOrWhiteSpace(IdentificationNumber)
                ? "El número de identificación es requerido"
                : string.Empty;

            // Validar Nombre (solo requerido)
            FirstNameError = string.IsNullOrWhiteSpace(FirstName)
                ? "El nombre es requerido"
                : string.Empty;

            // Validar Apellido (solo requerido)
            LastNameError = string.IsNullOrWhiteSpace(LastName)
                ? "El apellido es requerido"
                : string.Empty;

            // Validar Email (requerido y formato básico)
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "El email es requerido";
            }
            else if (!_emailRegex.IsMatch(Email))
            {
                EmailError = "El formato del email no es válido (ejemplo: usuario@dominio.com)";
            }
            else
            {
                EmailError = string.Empty;
            }

            // Validar Teléfono (solo requerido)
            PhoneError = string.IsNullOrWhiteSpace(Phone)
                ? "El teléfono es requerido"
                : string.Empty;            
        }        

        private void LoadUserData(UserDto user)
        {
            IdentificationNumber = user.IdentificationNumber;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Phone = user.Phone;
        }

        private void ClearForm()
        {
            IdentificationNumber = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            IsEditing = false;
            SelectedUser = null;

            // Limpiar errores
            IdentificationNumberError = string.Empty;
            FirstNameError = string.Empty;
            LastNameError = string.Empty;
            EmailError = string.Empty;
            PhoneError = string.Empty;

            // Notificar cambios
            OnPropertyChanged(nameof(CanSaveUser));
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged(nameof(IdentificationNumberError));
            OnPropertyChanged(nameof(FirstNameError));
            OnPropertyChanged(nameof(LastNameError));
            OnPropertyChanged(nameof(EmailError));
            OnPropertyChanged(nameof(PhoneError));

            SaveUserCommand.NotifyCanExecuteChanged();
        }

        // Implementación de IDataErrorInfo para validación
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(IdentificationNumber):
                        return string.IsNullOrWhiteSpace(IdentificationNumber) ? "Requerido" : string.Empty;
                    case nameof(FirstName):
                        return string.IsNullOrWhiteSpace(FirstName) ? "Requerido" : string.Empty;
                    case nameof(LastName):
                        return string.IsNullOrWhiteSpace(LastName) ? "Requerido" : string.Empty;
                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                            return "Requerido";
                        return !_emailRegex.IsMatch(Email) ? "Formato inválido" : string.Empty;
                    case nameof(Phone):
                        return string.IsNullOrWhiteSpace(Phone) ? "Requerido" : string.Empty;
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
