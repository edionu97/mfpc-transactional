using System;
using System.Data;

namespace DatabaseSystem.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class MapAttribute : Attribute
    {
        public string MapTo { get; }

        public SqlDbType Type { get; }

        public bool IsKey { get; }

        public MapAttribute(string mapTo, SqlDbType type, bool isKey = false)
        {
            MapTo = mapTo;
            Type = type;
            IsKey = isKey;
        }
    }
}
