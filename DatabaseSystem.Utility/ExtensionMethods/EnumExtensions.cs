using System;
using System.Collections.Generic;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Utility.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static IList<LockType> GetOpposite(this LockType @lock)
        {
            return @lock switch
            {
                LockType.Read => new List<LockType> { LockType.Write },
                LockType.Write => new List<LockType> { LockType.Read, LockType.Write },
                _ => throw new Exception("Undefined")
            };
        }
    }
}
