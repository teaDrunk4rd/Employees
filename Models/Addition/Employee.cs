using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using Employees.Classes;
using Employees.Models;
using LinqToDB;

namespace DataModels
{
    public partial class Employee : ViewModelBase, ICloneable, ISearchable
    {
        public string FullName => $"{Surname} {Name} {Patronymic}";

        public string SurnameInitials =>
            $"{Surname} {Name.First()}. {(!Patronymic.IsEmpty() ? Patronymic.First() : '\0')}{(!Patronymic.IsEmpty() ? '.' : '\0')}";
        
        public List<EmployeeSkill> SkillsToAdd { get; set; } = new List<EmployeeSkill>();
        
        public List<EmployeeSkill> SkillsToDelete { get; set; } = new List<EmployeeSkill>();

        public ObservableCollection<EmployeeSkill> Skills { get; set; }

        public void LoadSkills()
        {
            Skillidfks.ForEach(es => es.Skill = DBModel.SkillsTable.Find(es.SkillId));
        }

        public void AddSkill( Skill skill, short level)
        {
            var employeeSkill = new EmployeeSkill
            {
                Employee = this,
                EmployeeId = Id,
                Skill = skill,
                SkillId = skill.Id,
                Level = level
            };
            var index = SkillsToDelete.IndexOf(s => s == employeeSkill);
            if (index == -1)
                SkillsToAdd.Add(employeeSkill);
            else
                SkillsToDelete.RemoveAt(index);
        }

        public void UpdateSkills()
        {
            var skills = new List<EmployeeSkill>();
            if (!Skillidfks.IsEmpty() && !SkillsToAdd.IsEmpty())
                skills = Skillidfks.Union(SkillsToAdd).ToList();
            else if (!Skillidfks.IsEmpty())
                skills = Skillidfks.ToList();
            else if (!SkillsToAdd.IsEmpty())
                skills = SkillsToAdd;
            
            Skills = new ObservableCollection<EmployeeSkill>(skills.Where(es => SkillsToDelete.All(s => s != es)));
            RaisePropertyChanged(nameof(Skills));
        }

        public void SaveSkills(long id=0)
        {
            SkillsToAdd.ForEach(s =>
            {
                if (SkillsToDelete.Any(sd => sd == s)) return;
                if (id != 0)
                    s.EmployeeId = id;
                DBModel.Context.Insert(s);
            });
            SkillsToDelete.ForEach(s =>
            {
                if (s.EmployeeId != 0)
                    DBModel.EmployeesDB.Delete(s);
            });
        }
        
        public object Clone() => MemberwiseClone();
        
        public bool Search(string search) 
            => search.IsEmpty() || FullName.Search(search) || Phone.Search(search) 
               || Address.Search(search) || PassportNumberSeries.Search(search) 
               || PassportInfoWhom.Search(search) || PassportInfoWhen.Search(search) 
               || Department.Name.Search(search) || Position.Name.Search(search);
    }
}