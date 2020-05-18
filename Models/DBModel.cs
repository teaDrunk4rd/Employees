using DataModels;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Models
{
    class DBModel
    {
        public static EmployeesDB EmployeesDB { get; } = new EmployeesDB();
        
        public static DataContext Context { get; } = new DataContext();

        public static ITable<Employee> EmployeesTable { get; } 
            = EmployeesDB.GetTable<Employee>().LoadWith(e => e.Department).LoadWith(e => e.Position); // TODO: грузить скилы

        public static ITable<Department> DepartmentsTable { get; } = EmployeesDB.GetTable<Department>();

        public static ITable<Position> PositionsTable { get; } = EmployeesDB.GetTable<Position>();

        public static ITable<Skill> SkillsTable { get; } = EmployeesDB.GetTable<Skill>();
    }
}
