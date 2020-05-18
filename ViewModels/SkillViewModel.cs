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
    public class SkillViewModel : LookupViewModel
    {
        private Skill _selectedSkill;
        private Skill _skill;
        private List<Skill> _skills;
        private ObservableCollection<Skill> _filteredSkills;

        public Skill SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                if (Equals(_selectedSkill, value)) return;
                _selectedSkill = value;
                RaisePropertyChanged(nameof(SelectedSkill));
            }
        }

        public Skill Skill
        {
            get => _skill;
            set
            {
                if (Equals(_skill, value)) return;
                _skill = value;
                RaisePropertyChanged(nameof(Skill));
            }
        }

        public List<Skill> Skills
        {
            get => _skills;
            set
            {
                if (Equals(_skills, value)) return;
                _skills = value;
                RaisePropertyChanged(nameof(Skills));
            }
        }

        public ObservableCollection<Skill> FilteredSkills
        {
            get => _filteredSkills;
            set
            {
                if (Equals(_filteredSkills, value)) return;
                _filteredSkills = value;
                RaisePropertyChanged(nameof(FilteredSkills));
            }
        }

        public SkillViewModel()
        {
            Skills = DBModel.SkillsTable.ToList();
            FilteredSkills =
                new ObservableCollection<Skill>(Skills.Where(d => d.Search(Search)).OrderBy(d => d.Name));
        }

        public ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Skill = new Skill();
        }, () =>  Mode == WindowMode.Read);

        public ICommand ShowEditForm => new DelegateCommand(() => 
        {
            Mode = WindowMode.Edit;
            Skill = (Skill) SelectedSkill.Clone();
        }, 
        () => Mode == WindowMode.Read && SelectedSkill != default);

        public ICommand AddCommand => new DelegateCommand(() =>
        {
            DBModel.Context.Insert(Skill);
            ClearWithUpdate();
            SelectedSkill = FilteredSkills.Aggregate((d1, d2) => d1.Id > d2.Id ? d1 : d2);
            OnSelection?.Execute(this);
        }, () => CanExecuteUpsertCommand(Skill));

        public ICommand EditCommand => new DelegateCommand(() =>
        {
            SelectedSkill = (Skill) Skill.Clone();
            DBModel.EmployeesDB.Update(SelectedSkill);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(Skill));

        public ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationDialog() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedSkill);
            Skills.Remove(SelectedSkill);
            FilteredSkills.Remove(SelectedSkill);
            SelectedSkill = default;
            Clear();
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedSkill != default);

        public ICommand ClearCommand => new DelegateCommand(() =>
        {
            Clear();
            if (OnSelection != default)
                OnSelection = default;
        });

        private void Clear()
        {
            Skill = default;
            Mode = WindowMode.Read;
        }

        private void ClearWithUpdate()
        {
            Clear();
            Skills = DBModel.SkillsTable.ToList();
            UpdateSkills();
            OnUpdateCollection?.Execute(null);
        }

        private void UpdateSkills()
        {
            var selectedId = SelectedSkill?.Id;
            FilteredSkills =
                new ObservableCollection<Skill>(Skills.Where(d => d.Search(Search)).OrderBy(d => d.Name));
            if (selectedId != default)
                SelectedSkill = FilteredSkills.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Skill skill)
            => skill != null && !skill.Name.IsEmpty() &&
               !Skills.Any(d => d.Name == skill.Name && d.Id != skill.Id);

        protected override void RaiseSearchChanged() => UpdateSkills();
    }
}
