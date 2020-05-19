using DataModels;
using LinqToDB;

namespace Employees.Models
{
    static class DBModel
    {
        public static EmployeesDB EmployeesDB { get; private set; }
        
        public static DataContext Context { get; private set; }

        public static ITable<Employee> EmployeesTable { get; private set; }

        public static ITable<Department> DepartmentsTable { get; private set; }

        public static ITable<Position> PositionsTable { get; private set; }

        public static ITable<Skill> SkillsTable { get; private set; }

        static DBModel()
        {
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            EmployeesDB = new EmployeesDB();
            Context = new DataContext();
            EmployeesTable = EmployeesDB.GetTable<Employee>()
                .LoadWith(e => e.Department)
                .LoadWith(e => e.Position)
                .LoadWith(e => e.Skillidfks);
            DepartmentsTable = EmployeesDB.GetTable<Department>();
            PositionsTable = EmployeesDB.GetTable<Position>();
            SkillsTable = EmployeesDB.GetTable<Skill>();
        }
    }
}
