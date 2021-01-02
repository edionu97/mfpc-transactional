using System;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Utility.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static LockType GetOpposite(this LockType @lock)
        {
            return @lock switch
            {
                LockType.Read => LockType.Write,
                LockType.Write => LockType.Read,
                _ => throw new Exception("Undefined")
            };
        }
    }
}
