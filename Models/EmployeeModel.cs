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
            Id = employee.Id;
            FullName = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
            Phone = employee.Phone;
            Address = employee.Address;
            PassportNumberSeries = employee.PassportNumberSeries;
            PassportInfo = $"{employee.PassportInfoWhen:dd.MM.yyyy} {employee.PassportInfoWhom}";
            Department = employee.Department;
            Position = employee.Position;
        }
    }
}