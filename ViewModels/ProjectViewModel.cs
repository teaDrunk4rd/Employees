using System;
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
using Employees.Views;
using LinqToDB;

namespace Employees.ViewModels
{
    public class ProjectViewModel : LookupViewModel
    {
        private Project _selectedProject;
        private Project _project;
        private ProjectRequiredSkill _projectRequiredSkill;
        private ProjectParticipant _projectParticipant;
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

        public ProjectRequiredSkill SelectedProjectRequiredSkill
        {
            get => _projectRequiredSkill;
            set
            {
                if (Equals(_projectRequiredSkill, value)) return;
                _projectRequiredSkill = value;
                RaisePropertyChanged(nameof(SelectedProjectRequiredSkill));
            }
        }

        public ProjectParticipant SelectedProjectParticipant
        {
            get => _projectParticipant;
            set
            {
                if (Equals(_projectParticipant, value)) return;
                _projectParticipant = value;
                RaisePropertyChanged(nameof(SelectedProjectParticipant));
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
        
        private MainViewModel Parent { get; set; }

        public ProjectViewModel(MainViewModel parent)
        {
            Parent = parent;
            
            Projects = DBModel.ProjectsTable.ToList();
            FilteredProjects =
                new ObservableCollection<Project>(Projects.Where(d => d.Search(Search)).OrderBy(d => d.Name));
        }

        public override ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Project = new Project {StartDate = DateTime.Now, FinishDate = DateTime.Now};
        }, () =>  Mode == WindowMode.Read);

        public override ICommand ShowEditForm => new DelegateCommand(() => 
        {
            Mode = WindowMode.Edit;
            Project = (Project) SelectedProject.Clone();
            Project.UpdateSkills();
            Project.UpdateParticipants();
        }, 
        () => Mode == WindowMode.Read && SelectedProject != null);

        public override ICommand AddCommand => new DelegateCommand(() =>
        {
            var id = DBModel.Context.InsertWithInt64Identity(Project);
            Project.SaveSkills(id);
            Project.SaveParticipants(id);
            ClearWithUpdate();
            SelectedProject = FilteredProjects.First(d => d.Id == id);
        }, () => CanExecuteUpsertCommand(Project));

        public override ICommand EditCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Update(Project);
            Project.SaveSkills();
            Project.SaveParticipants();
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
        
        public ICommand OpenSkillsWindowForAdd => new DelegateCommand(() =>
        {
            Parent.SkillsViewModel.SelectedSkill = null;
            Parent.SkillsViewModel.SetFilter(
                skills => new ObservableCollection<Skill>(
                    skills.Where(s => Project.Skills.All(x => x.SkillId != s.Id))
                )
            );
            Parent.SkillsViewModel.OpenWindow<SkillViewModel, SkillView>(new DelegateCommand(() =>
            {
                var skillLevelChooserViewModel = new SkillLevelChooserViewModel { SkillName = Parent.SkillsViewModel.SelectedSkill.Name };
                skillLevelChooserViewModel.OpenModal<SkillLevelChooserViewModel, SkillLevelChooserView>(
                    new DelegateCommand(() =>
                    {
                        skillLevelChooserViewModel.CloseWindow();
                        Parent.SkillsViewModel.CloseWindow();
                        Parent.SkillsViewModel.RemoveFilter();
                        
                        Project.AddSkill(Parent.SkillsViewModel.SelectedSkill, skillLevelChooserViewModel.Level);
                        Project.UpdateSkills();
                    })
                );
            }));
        });

        public ICommand DeleteSkill => new DelegateCommand(() =>
        {
            Project.SkillsToDelete.Add((ProjectRequiredSkill) SelectedProjectRequiredSkill.Clone());
            SelectedProjectRequiredSkill = null;
            Project.UpdateSkills();
        }, () =>  SelectedProjectRequiredSkill != null);
        
        public ICommand OpenEmployeesWindowForAdd => new DelegateCommand(() =>
        {
            if (!Project.Participants.IsEmpty() || !Project.Skills.IsEmpty())
                Parent.EmployeeViewModel.SetFilter(
                    employees => new ObservableCollection<Employee>(
                        employees.Where(e => 
                            (Project.Participants.IsEmpty() || Project.Participants.All(x => x.EmployeeId != e.Id)) &&
                            (Project.Skills.IsEmpty() || !e.IsInProject(Project.StartDate) && e.HaveRequiredSkills(Project.Skills))
                        )
                    )
                );
            Parent.EmployeeViewModel.OpenWindow<EmployeeViewModel, EmployeeView>(new DelegateCommand(() =>
            {
                Parent.EmployeeViewModel.CloseWindow();
                Parent.EmployeeViewModel.RemoveFilter();
                
                Project.AddParticipant(Parent.EmployeeViewModel.SelectedEmployee);
                Project.UpdateParticipants();
            }));
        });

        public ICommand DeleteParticipant => new DelegateCommand(() =>
        {
            Project.ParticipantsToDelete.Add((ProjectParticipant) SelectedProjectParticipant.Clone());
            SelectedProjectParticipant = null;
            Project.UpdateParticipants();
        }, () =>  SelectedProjectParticipant != null);

        private void Clear()
        {
            Project = null;
            // SelectedProject?.ClearSkills();
            // SelectedProject?.ClearParticipants();
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Projects = DBModel.ProjectsTable.ToList();
            UpdateCollection();
            OnUpdateCollection?.Execute(null);
        }
        
        public override void UpdateCollection()
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
    }
}