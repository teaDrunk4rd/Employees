using System;
using DataModels;

namespace Employees.Models
{
    public class EmployeeModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PassportNumberSeries { get; set; }
        public string PassportInfo { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }

        public EmployeeModel(Employee employee)
        {
            if (employee == null) return;

            Id = employee.Id;
            FullName = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
            Phone = employee.Phone;
            Address = employee.Address;
            PassportNumberSeries = employee.PassportNumberSeries;
            PassportInfo = $"{employee.PassportInfoWhen:dd.MM.yyyy} {employee.PassportInfoWhom}";
            Department = employee.Department;
            Position = employee.Position;
        }

        public Employee GetEmployee()
        {
            var fullName = FullName.Split();
            var passportInfo = PassportInfo.Split(new[] {' '}, 2);
            return new Employee
            {
                Id = Id,
                Surname = fullName[0],
                Name = fullName[1],
                Patronymic = fullName.Length == 3 ? fullName[2] : null,
                Phone = Phone,
                Address = Address,
                PassportNumberSeries = PassportNumberSeries,
                PassportInfoWhen = DateTime.Parse(passportInfo[0]),
                PassportInfoWhom = passportInfo[1],
                Department = Department,
                DepartmentId = Department?.Id,
                Position = Position,
                PositionId = Position?.Id
            };
        }
    }
}