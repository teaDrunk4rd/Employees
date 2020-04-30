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
        private Department _appendableDepartment;
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

        public Department AppendableDepartment
        {
            get => _appendableDepartment;
            set
            {
                if (Equals(_appendableDepartment, value)) return;
                _appendableDepartment = value;
                RaisePropertyChanged(nameof(AppendableDepartment));
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

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            AppendableDepartment = new Department();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() => Mode = WindowMode.Edit, 
            () => Mode == WindowMode.Read && SelectedDepartment != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            DBModel.Context.Insert(AppendableDepartment);
            Clear();
            SelectedDepartment = Departments.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
        }, () => CanExecuteUpsertCommand(AppendableDepartment));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Update(SelectedDepartment);
            Clear();
        }, () => CanExecuteUpsertCommand(SelectedDepartment));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Delete(SelectedDepartment);
            Clear();
            SelectedDepartment = default;
        }, () =>  Mode == WindowMode.Read && SelectedDepartment != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            UpdateDepartments();
            Mode = WindowMode.Read;
        }

        private void UpdateDepartments()
        {
            var selectedId = SelectedDepartment?.Id;
            Departments = new ObservableCollection<Department>(DBModel.DepartmentsTable.ToList().OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Department department)
            => department != null && !department.Name.IsEmpty() &&
               !Departments.Any(d => d.Name == department.Name && d.Id != department.Id);
    }
}