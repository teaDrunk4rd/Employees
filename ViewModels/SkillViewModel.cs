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

        public override ICommand ShowAddForm => new DelegateCommand(() =>
        {
            Mode = WindowMode.Add;
            Skill = new Skill();
        }, () =>  Mode == WindowMode.Read);

        public override ICommand ShowEditForm => new DelegateCommand(() => 
        {
            Mode = WindowMode.Edit;
            Skill = (Skill) SelectedSkill.Clone();
        }, 
        () => Mode == WindowMode.Read && SelectedSkill != null);

        public override ICommand AddCommand => new DelegateCommand(() =>
        {
            var id = DBModel.Context.InsertWithInt64Identity(Skill);
            ClearWithUpdate();
            SelectedSkill = FilteredSkills.First(d => d.Id == id);
            OnSelection?.Execute(this);
        }, () => CanExecuteUpsertCommand(Skill));

        public override ICommand EditCommand => new DelegateCommand(() =>
        {
            DBModel.EmployeesDB.Update(Skill);
            ClearWithUpdate();
        }, () => CanExecuteUpsertCommand(Skill));

        public override ICommand DeleteCommand => new DelegateCommand(() =>
        {
            if (Extensions.ShowConfirmationModal() != MessageBoxResult.Yes) 
                return;
            DBModel.EmployeesDB.Delete(SelectedSkill);
            Skills.Remove(SelectedSkill);
            FilteredSkills.Remove(SelectedSkill);
            SelectedSkill = null;
            Clear();
            OnUpdateCollection?.Execute(null);
        }, () =>  Mode == WindowMode.Read && SelectedSkill != null);

        public ICommand ClearCommand => new DelegateCommand(Clear);

        private void Clear()
        {
            Skill = null;
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
            if (selectedId != null)
                SelectedSkill = FilteredSkills.FirstOrDefault(d => d.Id == selectedId);
        }

        private bool CanExecuteUpsertCommand(Skill skill)
            => skill != null && !skill.Name.IsEmpty() &&
               !Skills.Any(d => d.Name == skill.Name && d.Id != skill.Id);

        protected override void RaiseSearchChanged() => UpdateSkills();
    }
}
