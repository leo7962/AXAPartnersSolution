using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserManagementViewModel userManagementViewModel;

        [ObservableProperty]
        private DepartmentAssignmentViewModel departmentAssignmentViewModel;

        [ObservableProperty]
        private UserAssignmentsViewModel userAssignmentsViewModel;

        public MainViewModel(UserManagementViewModel userManagementViewModel,
                           DepartmentAssignmentViewModel departmentAssignmentViewModel,
                           UserAssignmentsViewModel userAssignmentsViewModel)
        {
            UserManagementViewModel = userManagementViewModel;
            DepartmentAssignmentViewModel = departmentAssignmentViewModel;
            UserAssignmentsViewModel = userAssignmentsViewModel;
        }
    }
}
