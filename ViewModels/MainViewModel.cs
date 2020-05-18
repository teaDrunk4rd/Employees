using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.Classes;
using Employees.Views;

namespace Employees.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public EmployeeViewModel EmployeeViewModel { get; private set; }
        
        public PositionViewModel PositionViewModel { get; private set; }
        
        public DepartmentViewModel DepartmentViewModel { get; private set; }

        public MainViewModel()
        {
            PositionViewModel = new PositionViewModel();
            DepartmentViewModel = new DepartmentViewModel();
            EmployeeViewModel = new EmployeeViewModel(this);
        }
        
        public ICommand OpenEmployeeWindow => new DelegateCommand(() =>
        {
            EmployeeViewModel.SelectedEmployeeModel = default;
            EmployeeViewModel.Search = default;
            EmployeeViewModel.OpenWindow<EmployeeViewModel, EmployeeView>();
        });
        
        public ICommand OpenPositionWindow => new DelegateCommand(() =>
        {
            PositionViewModel.SelectedPosition = default;
            PositionViewModel.Search = default;
            PositionViewModel.OpenWindow<PositionViewModel, PositionView>();
        });
        
        public ICommand OpenDepartmentWindow => new DelegateCommand(() =>
        {
            DepartmentViewModel.SelectedDepartment = default;
            DepartmentViewModel.Search = default;
            DepartmentViewModel.OpenWindow<DepartmentViewModel, DepartmentView>();
        });
    }
}