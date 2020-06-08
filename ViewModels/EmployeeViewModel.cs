﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm;
using Employees.Views;
using System.Windows.Input;
using DataModels;
using DevExpress.Mvvm.Native;
using Employees.Classes;
using Employees.Models;
using Employees.ViewModels.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    public class EmployeeViewModel : LookupViewModel
    {
        private Employee _selectedEmployee;
        private Employee _employee;
        private List<Employee> _employees;
        private ObservableCollection<EmployeeModel> _filteredEmployees;

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
            get => FilteredEmployees.FirstOrDefault(e => e.Id == _selectedEmployee?.Id);
            set
            {
                SelectedEmployee = value?.GetEmployee();
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

        public EmployeeSkill SelectedEmployeeSkill { get; set; }

        public List<Employee> Employees
        {
            get => _employees;
            set
            {
                if (Equals(_employees, value)) return;
                _employees = value;
                RaisePropertyChanged(nameof(Employees));
            }
        }

        public ObservableCollection<EmployeeModel> FilteredEmployees
        {
            get => _filteredEmployees;
            set
            {
                if (Equals(_filteredEmployees, value)) return;
                _filteredEmployees = value;
                RaisePropertyChanged(nameof(FilteredEmployees));
            }
        }
        
        public MainViewModel Parent { get; set; }

        public EmployeeViewModel(MainViewModel parent)
        {
            Parent = parent;
            Parent.PositionViewModel.OnUpdateCollection = new DelegateCommand(() =>
            {
                Employees = DBModel.EmployeesTable.ToList();
                UpdateEverything();
            });
            Parent.DepartmentViewModel.OnUpdateCollection = new DelegateCommand(() =>
            {
                Employees = DBModel.EmployeesTable.ToList();
                UpdateEverything();
            });
            
            Employees = DBModel.EmployeesTable.ToList();
            FilteredEmployees = new ObservableCollection<EmployeeModel>(Employees.Where(e => e.Search(Search))
                .Select(e => new EmployeeModel(e))
                .OrderBy(e => e.FullName)
            );
        }

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Employee = new Employee{PassportInfoWhen = DateTime.Today};
            Employee.UpdateSkills();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() =>
        {
            SelectedEmployee.Skillidfks.ForEach(es =>
                es.Skill = Parent.SkillsViewModel.Skills.FirstOrDefault(s=>s.Id == es.SkillId));
            Employee = (Employee) SelectedEmployee.Clone();
            Employee.UpdateSkills();
            Mode = WindowMode.Edit;
            UpdatePosition();
            UpdateDepartment();
        }, 
        () => Mode == WindowMode.Read && SelectedEmployee != default);

        public override ICommand AddCommand => new DelegateCommand(() =>
        {
            var id = DBModel.Context.InsertWithInt64Identity(Employee);
            Employee.SaveSkills(id);
            ClearWithUpdate();
            SelectedEmployeeModel = FilteredEmployees.First(d => d.Id == id);
        }, () => CanExecuteUpsertCommand(Employee));

        public override ICommand EditCommand => new DelegateCommand(() =>
        {
            Employee.PassportNumberSeries = Employee.PassportNumberSeries.Replace(" ", string.Empty); // TODO: использовать конвертер
            DBModel.EmployeesDB.Update(Employee);
            Employee.SaveSkills();
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(Employee));

        public override ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationModal() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedEmployee);
            Employees.Remove(Employees.First(e => e.Id == SelectedEmployee.Id));
            FilteredEmployees.Remove(SelectedEmployeeModel);
            SelectedEmployee = default;
            Clear();
        }, () =>  Mode == WindowMode.Read && SelectedEmployee != default);

        public ICommand ClearCommand => new DelegateCommand(Clear);
        
        public ICommand OpenPositionWindowForAdd => new DelegateCommand(() =>
        {
            Parent.PositionViewModel.SelectedPosition = default;
            Parent.PositionViewModel.OpenWindow<PositionViewModel, PositionView>(new DelegateCommand(() =>
            {
                Employee.Position = Parent.PositionViewModel.SelectedPosition;
                RaisePropertyChanged(nameof(Employee));
                Parent.PositionViewModel.CloseWindow();
            }), Parent.PositionViewModel.ShowAddForm);
        });
        
        public ICommand OpenDepartmentWindowForAdd => new DelegateCommand(() =>
        {
            Parent.DepartmentViewModel.SelectedDepartment = default;
            Parent.DepartmentViewModel.OpenWindow<DepartmentViewModel, DepartmentView>(new DelegateCommand(() =>
            {
                Employee.Department = Parent.DepartmentViewModel.SelectedDepartment;
                RaisePropertyChanged(nameof(Employee));
                Parent.DepartmentViewModel.CloseWindow();
            }), Parent.DepartmentViewModel.ShowAddForm);
        });
        
        public ICommand OpenSkillsWindowForAdd => new DelegateCommand(() =>
        {
            Parent.SkillsViewModel.SelectedSkill = default;
            Parent.SkillsViewModel.OpenWindow<SkillViewModel, SkillView>(new DelegateCommand(() =>
            {
                var skillLevelChooserViewModel = new SkillLevelChooserViewModel
                    { SkillName = Parent.SkillsViewModel.SelectedSkill.Name };
                skillLevelChooserViewModel.OpenModal<SkillLevelChooserViewModel, SkillLevelChooserView>(
                    new DelegateCommand(() =>
                    {
                        skillLevelChooserViewModel.CloseWindow();
                        Parent.SkillsViewModel.CloseWindow();
                        
                        Employee.SkillsToAdd.Add(new EmployeeSkill
                        {
                            Employee = Employee, 
                            EmployeeId = Employee.Id,
                            Level = skillLevelChooserViewModel.Level,
                            Skill = Parent.SkillsViewModel.SelectedSkill,
                            SkillId = Parent.SkillsViewModel.SelectedSkill.Id
                        });
                        Employee.UpdateSkills();
                    })
                );
            }));
        });

        public ICommand DeleteSkill => new DelegateCommand(() =>
        {
            Employee.SkillsToDelete.Add((EmployeeSkill) SelectedEmployeeSkill.Clone());
            SelectedEmployeeSkill = default;
            RaisePropertyChanged(nameof(SelectedEmployeeSkill));
            Employee.UpdateSkills();
        }, () =>  SelectedEmployeeSkill != default);

        private void Clear()
        {
            Employee = default;
            if (SelectedEmployee != default)
            {
                SelectedEmployee.SkillsToAdd = new List<EmployeeSkill>();
                SelectedEmployee.SkillsToDelete = new List<EmployeeSkill>();
            }
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Employees = DBModel.EmployeesTable.ToList();
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
            FilteredEmployees = new ObservableCollection<EmployeeModel>(Employees.Where(e => e.Search(Search))
                .Select(e => new EmployeeModel(e))
                .OrderBy(e => e.FullName)
            );
            if (selectedId != default)
                SelectedEmployeeModel = FilteredEmployees.FirstOrDefault(d => d.Id == selectedId);
        }

        private void UpdatePosition()
        {
            if (Mode == WindowMode.Read) return;
            
            if (Employee?.Position != default)
                Employee.Position = Parent.PositionViewModel.Positions.FirstOrDefault(p => p.Id == Employee.Position.Id);
            else if (Employee?.PositionId != default)
                Employee.Position = Parent.PositionViewModel.Positions.FirstOrDefault(p => p.Id == Employee.PositionId);
            RaisePropertyChanged(nameof(Employee));
        }
        
        private void UpdateDepartment()
        {
            if (Mode == WindowMode.Read) return;
            
            if (Employee?.Department != null)
                Employee.Department = Parent.DepartmentViewModel.Departments.FirstOrDefault(p => p.Id == Employee.Department.Id);
            else if (Employee?.DepartmentId != default)
                Employee.Department = Parent.DepartmentViewModel.Departments.FirstOrDefault(p => p.Id == Employee.DepartmentId);
            RaisePropertyChanged(nameof(Employee));
        }

        private bool CanExecuteUpsertCommand(Employee employee)
            => employee != null && !employee.Surname.IsEmpty() && !employee.Name.IsEmpty() && !employee.Phone.IsEmpty() 
               && !employee.Address.IsEmpty() && !employee.PassportNumberSeries.IsEmpty() && !employee.PassportInfoWhom.IsEmpty() 
               && employee.PassportInfoWhen != default;

        protected override void RaiseSearchChanged() => UpdateEmployees();
    }
}
