using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using Employees.Classes;
using Employees.Models;
using LinqToDB;

namespace DataModels
{
    public partial class Employee : ViewModelBase, ICloneable, ISearchable
    {
        public List<EmployeeSkill> SkillsToAdd = new List<EmployeeSkill>();
        public List<EmployeeSkill> SkillsToDelete = new List<EmployeeSkill>();

        public ObservableCollection<EmployeeSkill> Skills { get; set; }

        public string FullName => $"{Surname} {Name} {Patronymic}";

        public void UpdateSkills()
        {
            var skills = new List<EmployeeSkill>();
            if (!Skillidfks.Equals(default) && !SkillsToAdd.Equals(default))
                skills = Skillidfks.Union(SkillsToAdd).ToList();
            else if (!Skillidfks.Equals(default))
                skills = Skillidfks.ToList();
            else if (!SkillsToAdd.Equals(default))
                skills = SkillsToAdd;
            Skills = new ObservableCollection<EmployeeSkill>(skills.Where(es =>
                !SkillsToDelete.Any(s => s.SkillId == es.SkillId && s.EmployeeId == es.EmployeeId))); // TODO: перенести часть логики в EmployeeSkill
            RaisePropertyChanged(nameof(Skills));
        }

        public void SaveSkills(long id=0)
        {
            SkillsToAdd.ForEach(s =>
            {
                if (SkillsToDelete.Any(sd => sd.SkillId == s.SkillId && sd.EmployeeId == s.EmployeeId)) return; // TODO: перенести часть логики в EmployeeSkill
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