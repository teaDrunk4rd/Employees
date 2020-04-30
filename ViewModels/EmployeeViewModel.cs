using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Employees.Views;
using System.Windows.Input;
using DataModels;
using Employees.Classes;
using Employees.Models;
using LinqToDB;

namespace Employees.ViewModels
{
    class EmployeeViewModel : ViewModelBase
    {
        private WindowMode _mode;
        private Employee _selectedEmployee;
        private Employee _appendableEmployee;
        private ObservableCollection<EmployeeModel> _employees
            = new ObservableCollection<EmployeeModel>(DBModel.EmployeesTable.ToList()
                .Select(e => new EmployeeModel(e)).OrderBy(e => e.FullName));
        private ObservableCollection<Position> _positions // TODO: привязать модели
            = new ObservableCollection<Position>(DBModel.PositionsTable.ToList().OrderBy(d => d.Name));
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

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                if (Equals(_selectedEmployee, value)) return;
                _selectedEmployee = value;
                RaisePropertyChanged(nameof(SelectedEmployee));
            }
        }

        public EmployeeModel SelectedEmployeeModel
        {
            get => Employees.FirstOrDefault(e => e.Id == _selectedEmployee?.Id);
            set
            {
                var employee = value?.GetEmployee();
                if (Equals(SelectedEmployee, employee)) return;
                SelectedEmployee = employee;
                RaisePropertyChanged(nameof(SelectedEmployeeModel));
            }
        }

        public Employee AppendableEmployee
        {
            get => _appendableEmployee;
            set
            {
                if (Equals(_appendableEmployee, value)) return;
                _appendableEmployee = value;
                RaisePropertyChanged(nameof(AppendableEmployee));
            }
        }

        public ObservableCollection<EmployeeModel> Employees
        {
            get => _employees;
            set
            {
                if (Equals(_employees, value)) return;
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
            }
        }

        public ObservableCollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (Equals(_positions, value)) return;
                _positions = value;
                RaisePropertyChanged(nameof(Positions));
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
            AppendableEmployee = new Employee{PassportInfoWhen = DateTime.Today};
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Edit;
            UpdateDepartments(SelectedEmployee);
            UpdatePositions(SelectedEmployee);
            RaisePropertyChanged(nameof(SelectedEmployee));
        }, 
        () => Mode == WindowMode.Read && SelectedEmployee != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            AppendableEmployee.DepartmentId = AppendableEmployee.Department?.Id;
            AppendableEmployee.PositionId = AppendableEmployee.Position?.Id;
            DBModel.Context.Insert(AppendableEmployee);
            Clear();
            SelectedEmployee = Employees.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2).GetEmployee();
        }, () => CanExecuteUpsertCommand(AppendableEmployee));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedEmployee.DepartmentId = SelectedEmployee.Department?.Id;
            SelectedEmployee.PositionId = SelectedEmployee.Position?.Id;
            DBModel.EmployeesDB.Update(SelectedEmployee);
            Clear();
        }, () => CanExecuteUpsertCommand(SelectedEmployee));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Delete(SelectedEmployee);
            Clear();
            SelectedEmployee = default;
        }, () =>  Mode == WindowMode.Read && SelectedEmployee != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);
        
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

        private void Clear()
        {
            UpdateEmployees();
            Mode = WindowMode.Read;
        }

        private void UpdateEmployees()
        {
            var selectedId = SelectedEmployeeModel?.Id;
            Employees = new ObservableCollection<EmployeeModel>(DBModel.EmployeesTable.ToList()
                .Select(e => new EmployeeModel(e)).OrderBy(e => e.FullName));
            if (selectedId != default)
                SelectedEmployeeModel = Employees.FirstOrDefault(d => d.Id == selectedId);
        }

        private void UpdatePositions(Employee employee)
        {
            Positions = new ObservableCollection<Position>(DBModel.PositionsTable.ToList().OrderBy(d => d.Name));
            if (employee.Position != default)
                employee.Position = Positions.First(p => p.Id == employee.Position.Id);
        }

        private void UpdateDepartments(Employee employee)
        {
            Departments = new ObservableCollection<Department>(DBModel.DepartmentsTable.ToList().OrderBy(d => d.Name));
            if (employee.Department != default)
                employee.Department = Departments.First(p => p.Id == employee.Department.Id);
        }

        private bool CanExecuteUpsertCommand(Employee employee)
            => employee != null && !employee.Surname.IsEmpty() && !employee.Name.IsEmpty() && !employee.Phone.IsEmpty() 
               && !employee.Address.IsEmpty() && !employee.PassportNumberSeries.IsEmpty() && !employee.PassportInfoWhom.IsEmpty() 
               && employee.PassportInfoWhen != default;
    }
}
