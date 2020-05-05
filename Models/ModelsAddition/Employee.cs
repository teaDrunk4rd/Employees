using System;

namespace DataModels
{
    public partial class Employee : ICloneable
    {
        public object Clone() => MemberwiseClone();
    }
}