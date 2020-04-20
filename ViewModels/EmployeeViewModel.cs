using DevExpress.Mvvm;
using Employees.Views;
using System.Windows;
using System.Windows.Input;

namespace Employees.ViewModels
{
    class EmployeeViewModel : ViewModelBase
    {
        private bool _editMode;

        public bool EditMode
        {
            get => _editMode;
            set
            {
                _editMode = value;
                RaisePropertiesChanged(nameof(ListVisibility), nameof(EditFormVisibility));
            }
        }

        public Visibility ListVisibility => EditMode ? Visibility.Collapsed : Visibility.Visible;

        public Visibility EditFormVisibility => EditMode ? Visibility.Visible : Visibility.Collapsed;

        public ICommand AddCommand => new DelegateCommand(() => EditMode = !EditMode);

        public ICommand OpenDepartmentWindow => new DelegateCommand(() =>
        {
            var departmentWindow = new DepartmentWindow();
            departmentWindow.Show();
        });

        public ICommand OpenPositionWindow => new DelegateCommand(() =>
        {

            var positionWindow = new PositionWindow();
            positionWindow.Show();
        });
    }
}
