using System;

namespace DataModels
{
    public partial class EmployeeSkill : ICloneable
    {
        public object Clone() => MemberwiseClone();

        public static bool operator ==(EmployeeSkill es1, EmployeeSkill es2)
            => es1?.SkillId == es2?.SkillId && es1?.EmployeeId == es2?.EmployeeId;

        public static bool operator !=(EmployeeSkill es1, EmployeeSkill es2)
            => es1?.SkillId != es2?.SkillId || es1?.EmployeeId != es2?.EmployeeId;

        public override string ToString()
            => $"{Skill.Name}: {Level}";
    }
}