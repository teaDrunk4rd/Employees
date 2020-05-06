using System;
using Employees.Classes;
using Employees.Models;

namespace DataModels
{
    public partial class Employee : ICloneable, ISearchable
    {
        public object Clone() => MemberwiseClone();
        
        public bool Search(string search) 
            => search.IsEmpty() || FullName().Search(search) || Phone.Search(search) 
               || Address.Search(search) || PassportNumberSeries.Search(search) 
               || PassportInfoWhom.Search(search) || PassportInfoWhen.Search(search) 
               || Department.Name.Search(search) || Position.Name.Search(search);

        public string FullName() => $"{Surname} {Name} {Patronymic}";
    }
}