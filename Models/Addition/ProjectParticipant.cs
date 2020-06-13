using System;

namespace DataModels
{
    public partial class ProjectParticipant : ICloneable
    {
        public object Clone() => MemberwiseClone();

        public static bool operator ==(ProjectParticipant es1, ProjectParticipant es2)
            => es1?.EmployeeId == es2?.EmployeeId && es1?.ProjectId == es2?.ProjectId;

        public static bool operator !=(ProjectParticipant es1, ProjectParticipant es2)
            => es1?.EmployeeId != es2?.EmployeeId || es1?.ProjectId != es2?.ProjectId;
    }
}