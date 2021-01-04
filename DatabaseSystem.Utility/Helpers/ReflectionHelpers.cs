using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DatabaseSystem.Utility.Helpers
{
    public class ReflectionHelpers
    {
        public static Task<PropertyInfo> GetPropertyValueFromObjectAs<TObject, TAttribute>()
        {
            return Task.Run(() =>
            {
                var @object = Activator.CreateInstance(typeof(TObject));

                if (@object == null)
                {
                    return null;
                }

                foreach (var propertyInfo in @object.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(true)
                        .Any(customAttribute => customAttribute.GetType() == typeof(TAttribute)))
                    {
                        return propertyInfo;
                    }
                }

                return null;
            });
        }

        public static TAttribute GetCustomAttributeFromProperty<TAttribute>(PropertyInfo propertyInfo) where TAttribute: Attribute
        {
            return propertyInfo
                .GetCustomAttributes()
                .Where(attribute => attribute.GetType() == typeof(TAttribute))
                .Cast<TAttribute>()
                .FirstOrDefault();
        }
    }
}
