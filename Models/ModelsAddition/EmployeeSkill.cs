using System;

namespace DataModels
{
    public partial class EmployeeSkill : ICloneable
    {
        public object Clone() => MemberwiseClone();
    }
}