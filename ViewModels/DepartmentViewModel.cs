using DataModels;
using DevExpress.Mvvm;
using Employees.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Employees.ViewModels
{
    class DepartmentViewModel : ViewModelBase
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

        public ObservableCollection<Department> Departments { get; } = new ObservableCollection<Department>(DBModel.DepartmentsTable.ToList());

        public Visibility ListVisibility => EditMode ? Visibility.Collapsed : Visibility.Visible;

        public Visibility EditFormVisibility => EditMode ? Visibility.Visible : Visibility.Collapsed;

        public ICommand AddCommand => new DelegateCommand(() => EditMode = !EditMode);
    }
}
