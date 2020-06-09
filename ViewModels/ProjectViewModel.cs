using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataModels;
using DevExpress.Mvvm;
using Employees.Classes;
using Employees.Models;
using Employees.ViewModels.Classes;
using LinqToDB;

namespace Employees.ViewModels
{
    public class ProjectViewModel : LookupViewModel
    {
        private Project _selectedProject;
        private Project _project;
        private List<Project> _projects;
        private ObservableCollection<Project> _filteredProjects;

        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (Equals(_selectedProject, value)) return;
                _selectedProject = value;
                RaisePropertyChanged(nameof(SelectedProject));
            }
        }

        public Project Project
        {
            get => _project;
            set
            {
                if (Equals(_project, value)) return;
                _project = value;
                RaisePropertyChanged(nameof(Project));
            }
        }

        public List<Project> Projects
        {
            get => _projects;
            set
            {
                if (Equals(_projects, value)) return;
                _projects = value;
                RaisePropertyChanged(nameof(Projects));
            }
        }

        public ObservableCollection<Project> FilteredProjects
        {
            get => _filteredProjects;
            set
            {
                if (Equals(_filteredProjects, value)) return;
                _filteredProjects = value;
                RaisePropertyChanged(nameof(FilteredProjects));
            }
        }

        public ProjectViewModel()
        {
            Projects = DBModel.ProjectsTable.ToList();
            FilteredProjects =
                new ObservableCollection<Project>(Projects.Where(d => d.Search(Search)).OrderBy(d => d.Name));
        }

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Project = new Project();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() => 
        {
            Mode = WindowMode.Edit;
            Project = (Project) SelectedProject.Clone();
        }, 
        () => Mode == WindowMode.Read && SelectedProject != null);

        public override ICommand AddCommand => new DelegateCommand(() =>
        {
            var id = DBModel.Context.InsertWithInt64Identity(Project);
            ClearWithUpdate();
            SelectedProject = FilteredProjects.First(d => d.Id == id);
        }, () => CanExecuteUpsertCommand(Project));

        public override ICommand EditCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Update(Project);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(Project));

        public override ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationModal() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedProject);
            Projects.Remove(SelectedProject);
            FilteredProjects.Remove(SelectedProject);
            SelectedProject = null;
            Clear();
        }, () =>  Mode == WindowMode.Read && SelectedProject != null);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            Project = null;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Projects = DBModel.ProjectsTable.ToList();
            UpdateProjects();
            OnUpdateCollection?.Execute(null);
        }

        private void UpdateProjects()
        {
            var selectedId = SelectedProject?.Id;
            FilteredProjects =
                new ObservableCollection<Project>(Projects.Where(d => d.Search(Search)).OrderBy(d => d.Name));
            if (selectedId != null)
                SelectedProject = FilteredProjects.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Project project)
            => project != null && !project.Name.IsEmpty() &&
               !Projects.Any(d => d.Name == project.Name && d.Id != project.Id);

        protected override void RaiseSearchChanged() => UpdateProjects();
    }
}