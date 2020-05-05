using System;

namespace DataModels
{
    public partial class Position : ICloneable
    {
        public object Clone() => MemberwiseClone();
    }
}