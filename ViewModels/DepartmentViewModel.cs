using System.Collections.Generic;
using DataModels;
using DevExpress.Mvvm;
using Employees.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Employees.Classes;
using Employees.ViewModels.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    class DepartmentViewModel : LookupViewModel
    {
        private Department _selectedDepartment;
        private Department _department;
        private List<Department> _departments;
        private ObservableCollection<Department> _filteredDepartments;

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

        public List<Department> Departments
        {
            get => _departments;
            set
            {
                if (Equals(_departments, value)) return;
                _departments = value;
                RaisePropertyChanged(nameof(Departments));
            }
        }

        public ObservableCollection<Department> FilteredDepartments
        {
            get => _filteredDepartments;
            set
            {
                if (Equals(_filteredDepartments, value)) return;
                _filteredDepartments = value;
                RaisePropertyChanged(nameof(FilteredDepartments));
            }
        }

        public DepartmentViewModel()
        {
            Departments = DBModel.DepartmentsTable.ToList();
            FilteredDepartments =
                new ObservableCollection<Department>(Departments.Where(d => d.Search(Search)).OrderBy(d => d.Name));
        }

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
            SelectedDepartment = FilteredDepartments.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
            OnSelection?.Execute(this);
        }, () => CanExecuteUpsertCommand(Department));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedDepartment = (Department) Department.Clone();
            DBModel.EmployeesDB.Update(SelectedDepartment);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(SelectedDepartment));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationDialog() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedDepartment);
            Departments.Remove(SelectedDepartment);
            FilteredDepartments.Remove(SelectedDepartment);
            SelectedDepartment = default;
            Clear();
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedDepartment != default);

        public ICommand ClearCommand => new DelegateCommand(() =>
        {
            Clear();
            if (OnSelection != default)
                OnSelection = default;
        });

        private void Clear()
        {
            Department = default;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Departments = DBModel.DepartmentsTable.ToList();
            UpdateDepartments();
            OnUpdateCollection?.Execute(null);
        }

        private void UpdateDepartments()
        {
            var selectedId = SelectedDepartment?.Id;
            FilteredDepartments =
                new ObservableCollection<Department>(Departments.Where(d => d.Search(Search)).OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedDepartment = FilteredDepartments.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Department department)
            => department != null && !department.Name.IsEmpty() &&
               !Departments.Any(d => d.Name == department.Name && d.Id != department.Id);

        protected override void RaiseSearchChanged() => UpdateDepartments();
    }
}
