using System;

namespace DataModels
{
    public partial class Department : ICloneable
    {
        public object Clone() => MemberwiseClone();
    }
}