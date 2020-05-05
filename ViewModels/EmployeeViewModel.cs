using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Employees.Views;
using System.Windows.Input;
using DataModels;
using Employees.Classes;
using Employees.Models;
using Employees.ViewModels.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    class EmployeeViewModel : ViewModelBase
    {
        private WindowMode _mode;
        private Employee _selectedEmployee;
        private Employee _employee;
        private ObservableCollection<EmployeeModel> _employees
            = new ObservableCollection<EmployeeModel>(DBModel.EmployeesTable.ToList()
                .Select(e => new EmployeeModel(e)).OrderBy(e => e.FullName));

        public WindowMode Mode
        {
            get => _mode;
            set
            {
                if (Equals(_mode, value)) return;
                _mode = value;
                //RaisePropertyChanged(nameof(Mode));
                RaisePropertiesChanged(nameof(Mode), nameof(FormCommand), nameof(FormName), nameof(FormVisibility));
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

        public Employee Employee
        {
            get => _employee;
            set
            {
                if (Equals(_employee, value)) return;
                _employee = value;
                RaisePropertyChanged(nameof(Employee));
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

        // TODO: <fix this>
        public string FormName => Mode == WindowMode.Add ? "Добавление" : "Редактирование";
        
        public ICommand FormCommand => Mode == WindowMode.Add ? AddCommand : EditCommand;

        public Visibility FormVisibility => Mode == WindowMode.Read ? Visibility.Collapsed : Visibility.Visible;
        // </fix this>
        
        public PositionViewModel PositionViewModel { get; } = new PositionViewModel();
        
        public DepartmentViewModel DepartmentViewModel { get; } = new DepartmentViewModel();

        public EmployeeViewModel()
        {
            PositionViewModel.OnUpdateCollection = new DelegateCommand(UpdateEverything);
            DepartmentViewModel.OnUpdateCollection = new DelegateCommand(UpdateEverything);
        }

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Employee = new Employee{PassportInfoWhen = DateTime.Today};
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() =>
        {
            Employee = (Employee) SelectedEmployee.Clone();
            Mode = WindowMode.Edit;
            UpdatePosition();
            UpdateDepartment();
        }, 
        () => Mode == WindowMode.Read && SelectedEmployee != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            DBModel.Context.Insert(Employee);
            ClearWithUpdate();
            SelectedEmployee = Employees.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2).GetEmployee();
        }, () => CanExecuteUpsertCommand(Employee));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedEmployee = (Employee) Employee.Clone();
            DBModel.EmployeesDB.Update(SelectedEmployee);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(SelectedEmployee));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Delete(SelectedEmployee);
            Employees.Remove(SelectedEmployeeModel);
            SelectedEmployee = default;
            Mode = WindowMode.Read;
        }, () =>  Mode == WindowMode.Read && SelectedEmployee != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        public ICommand OpenPositionWindow => new DelegateCommand(() =>
        {
            PositionViewModel.OpenWindow<PositionViewModel, PositionWindow>();
        });
        
        public ICommand OpenDepartmentWindow => new DelegateCommand(() =>
        {
            DepartmentViewModel.OpenWindow<DepartmentViewModel, DepartmentWindow>();
        });
        
        public ICommand OpenPositionWindowForAdd => new DelegateCommand(() =>
        {
            PositionViewModel.OpenWindow<PositionViewModel, PositionWindow>(new DelegateCommand(() =>
            {
                Employee.Position = PositionViewModel.SelectedPosition;
                RaisePropertyChanged(nameof(Employee));
                PositionViewModel.CloseWindow();
            }));
        });
        
        public ICommand OpenDepartmentWindowForAdd => new DelegateCommand(() =>
        {
            DepartmentViewModel.OpenWindow<DepartmentViewModel, DepartmentWindow>(new DelegateCommand(() =>
            {
                Employee.Department = DepartmentViewModel.SelectedDepartment;
                RaisePropertyChanged(nameof(Employee));
                DepartmentViewModel.CloseWindow();
            }));
        });

        private void Clear()
        {
            Employee = default;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            UpdateEmployees();
        }

        private void UpdateEverything()
        {
            UpdateEmployees();
            UpdatePosition();
            UpdateDepartment();
        }

        private void UpdateEmployees()
        {
            var selectedId = SelectedEmployeeModel?.Id;
            Employees = new ObservableCollection<EmployeeModel>(DBModel.EmployeesTable.ToList()
                .Select(e => new EmployeeModel(e)).OrderBy(e => e.FullName));
            if (selectedId != default)
                SelectedEmployeeModel = Employees.FirstOrDefault(d => d.Id == selectedId);
        }

        private void UpdatePosition()
        {
            if (Mode == WindowMode.Read) return;
            
            if (Employee?.Position != default)
                Employee.Position = PositionViewModel.Positions.FirstOrDefault(p => p.Id == Employee.Position.Id);
            else if (Employee?.PositionId != default)
                Employee.Position = PositionViewModel.Positions.FirstOrDefault(p => p.Id == Employee.PositionId);
            RaisePropertyChanged(nameof(Employee));
        }
        
        private void UpdateDepartment()
        {
            if (Mode == WindowMode.Read) return;
            
            if (Employee?.Department != null)
                Employee.Department = DepartmentViewModel.Departments.FirstOrDefault(p => p.Id == Employee.Department.Id);
            else if (Employee?.DepartmentId != default)
                Employee.Department = DepartmentViewModel.Departments.FirstOrDefault(p => p.Id == Employee.DepartmentId);
            RaisePropertyChanged(nameof(Employee));
        }

        private bool CanExecuteUpsertCommand(Employee employee)
            => employee != null && !employee.Surname.IsEmpty() && !employee.Name.IsEmpty() && !employee.Phone.IsEmpty() 
               && !employee.Address.IsEmpty() && !employee.PassportNumberSeries.IsEmpty() && !employee.PassportInfoWhom.IsEmpty() 
               && employee.PassportInfoWhen != default;
    }
}
