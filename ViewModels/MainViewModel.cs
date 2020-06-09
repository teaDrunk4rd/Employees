﻿using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.Classes;
using Employees.Views;

namespace Employees.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public PositionViewModel PositionViewModel { get; }
        
        public DepartmentViewModel DepartmentViewModel { get; }
        
        public SkillViewModel SkillsViewModel { get; }
        
        public EmployeeViewModel EmployeeViewModel { get; }

        public ProjectViewModel ProjectViewModel { get; }

        public MainViewModel()
        {
            PositionViewModel = new PositionViewModel();
            DepartmentViewModel = new DepartmentViewModel();
            SkillsViewModel = new SkillViewModel();
            EmployeeViewModel = new EmployeeViewModel(this);
            ProjectViewModel = new ProjectViewModel();
        }
        
        public ICommand OpenEmployeeWindow => new DelegateCommand(() =>
        {
            EmployeeViewModel.SelectedEmployee = null;
            EmployeeViewModel.OpenWindow<EmployeeViewModel, EmployeeView>();
        });
        
        public ICommand OpenPositionWindow => new DelegateCommand(() =>
        {
            PositionViewModel.SelectedPosition = null;
            PositionViewModel.OpenWindow<PositionViewModel, PositionView>();
        });
        
        public ICommand OpenDepartmentWindow => new DelegateCommand(() =>
        {
            DepartmentViewModel.SelectedDepartment = null;
            DepartmentViewModel.OpenWindow<DepartmentViewModel, DepartmentView>();
        });
        
        public ICommand OpenSkillWindow => new DelegateCommand(() =>
        {
            SkillsViewModel.SelectedSkill = null;
            SkillsViewModel.OpenWindow<SkillViewModel, SkillView>();
        });
    }
}