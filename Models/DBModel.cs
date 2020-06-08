using DataModels;
using LinqToDB;

namespace Employees.Models
{
    static class DBModel
    {
        public static EmployeesDB EmployeesDB { get; }
        
        public static DataContext Context { get; }

        public static ITable<Employee> EmployeesTable { get; }

        public static ITable<Department> DepartmentsTable { get; }

        public static ITable<Position> PositionsTable { get; }

        public static ITable<Skill> SkillsTable { get; }

        public static ITable<Project> ProjectsTable { get; }

        static DBModel()
        {
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;
            EmployeesDB = new EmployeesDB();
            Context = new DataContext();
            EmployeesTable = EmployeesDB.Employees
                .LoadWith(e => e.Department)
                .LoadWith(e => e.Position)
                .LoadWith(e => e.Skillidfks);
            DepartmentsTable = EmployeesDB.Departments;
            PositionsTable = EmployeesDB.Positions;
            SkillsTable = EmployeesDB.Skills;
            ProjectsTable = EmployeesDB.Projects
                .LoadWith(e => e.Participantidfks)
                .LoadWith(e => e.Requiredskillsidfks);
        }
    }
}
