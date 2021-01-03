using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DatabaseSystem.Utility.ExtensionMethods
{
    public static class ObjectExtensions
    {
        public static TU GetValue<T, TU>(this ConcurrentDictionary<T, TU> dictionary, T key)
        {
            TU value;
            while (!dictionary.TryGetValue(key, out value))
            {
            }

            return value;
        }
    }
}
