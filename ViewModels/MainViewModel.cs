using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using Employees.Classes;
using Employees.Models;
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
            PositionViewModel.OnUpdateCollection = new DelegateCommand(() => // TODO: превратить в initialize
            {
                EmployeeViewModel.Employees = DBModel.EmployeesTable.ToList();
                EmployeeViewModel.UpdateEverything();
            });
            DepartmentViewModel.OnUpdateCollection = new DelegateCommand(() =>
            {
                EmployeeViewModel.Employees = DBModel.EmployeesTable.ToList();
                EmployeeViewModel.UpdateEverything();
            }); // TODO: skills OnUpdateCollection
            ProjectViewModel = new ProjectViewModel(this);
        }
        
        public ICommand OpenEmployeeWindow => new DelegateCommand(() =>
        {
            EmployeeViewModel.SelectedEmployee = null;
            EmployeeViewModel.RemoveFilter();
            EmployeeViewModel.OpenWindow<EmployeeViewModel, EmployeeView>();
        });
        
        public ICommand OpenPositionWindow => new DelegateCommand(() =>
        {
            PositionViewModel.SelectedPosition = null;
            PositionViewModel.RemoveFilter();
            PositionViewModel.OpenWindow<PositionViewModel, PositionView>();
        });
        
        public ICommand OpenDepartmentWindow => new DelegateCommand(() =>
        {
            DepartmentViewModel.SelectedDepartment = null;
            DepartmentViewModel.RemoveFilter();
            DepartmentViewModel.OpenWindow<DepartmentViewModel, DepartmentView>();
        });
        
        public ICommand OpenSkillWindow => new DelegateCommand(() =>
        {
            SkillsViewModel.SelectedSkill = null;
            SkillsViewModel.RemoveFilter();
            SkillsViewModel.OpenWindow<SkillViewModel, SkillView>();
        });
    }
}