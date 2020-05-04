using DataModels;
using DevExpress.Mvvm;
using Employees.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Employees.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    class DepartmentViewModel : ViewModelBase
    {
        private WindowMode _mode;
        private Department _selectedDepartment;
        private Department _department;
        private ObservableCollection<Department> _departments
            = new ObservableCollection<Department>(DBModel.DepartmentsTable.ToList().OrderBy(d => d.Name));

        public WindowMode Mode
        {
            get => _mode;
            set
            {
                if (Equals(_mode, value)) return;
                _mode = value;
                RaisePropertyChanged(nameof(Mode));
            }
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (Equals(_selectedDepartment, value)) return;
                _selectedDepartment = value;
                RaisePropertyChanged(nameof(SelectedDepartment));
            }
        }

        public Department Department
        {
            get => _department;
            set
            {
                if (Equals(_department, value)) return;
                _department = value;
                RaisePropertyChanged(nameof(Department));
            }
        }

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set
            {
                if (Equals(_departments, value)) return;
                _departments = value;
                RaisePropertyChanged(nameof(Departments));
            }
        }

        public ICommand OnUpdateCollection { get; set; }

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Department = new Department();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() => 
        {
            Mode = WindowMode.Edit;
            Department = (Department) SelectedDepartment.Clone();
        }, 
        () => Mode == WindowMode.Read && SelectedDepartment != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            DBModel.Context.Insert(Department);
            ClearWithUpdate();
            SelectedDepartment = Departments.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
        }, () => CanExecuteUpsertCommand(Department));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedDepartment = (Department) Department.Clone();
            DBModel.EmployeesDB.Update(SelectedDepartment);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(SelectedDepartment));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Delete(SelectedDepartment);
            Departments.Remove(SelectedDepartment);
            SelectedDepartment = default;
            Mode = WindowMode.Read;
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedDepartment != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            Department = default;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            UpdateDepartments();
        }

        private void UpdateDepartments()
        {
            var selectedId = SelectedDepartment?.Id;
            Departments = new ObservableCollection<Department>(DBModel.DepartmentsTable.ToList().OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == selectedId);
            OnUpdateCollection?.Execute(null);
        }

        private bool CanExecuteUpsertCommand(Department department)
            => department != null && !department.Name.IsEmpty() &&
               !Departments.Any(d => d.Name == department.Name && d.Id != department.Id);
    }
}
