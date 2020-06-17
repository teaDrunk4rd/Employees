using System;

namespace DataModels
{
    public partial class ProjectRequiredSkill : ICloneable
    {
        public object Clone() => MemberwiseClone();

        public static bool operator ==(ProjectRequiredSkill es1, ProjectRequiredSkill es2)
            => es1?.SkillId == es2?.SkillId && es1?.ProjectId == es2?.ProjectId && es1?.Level == es2?.Level;

        public static bool operator !=(ProjectRequiredSkill es1, ProjectRequiredSkill es2)
            => es1?.SkillId != es2?.SkillId || es1?.ProjectId != es2?.ProjectId || es1?.Level != es2?.Level;
    }
}