using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DatabaseSystem.Utility.Attributes;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Utility.Helpers
{
    public static class ConvertorHelpers
    {
        public static async IAsyncEnumerable<T> ConvertSqlRowsToObjects<T>(SqlDataReader sqlDataReader) where T : new()
        {
            //read table line by line
            while (await sqlDataReader.ReadAsync())
            {
                //create object
                var @object = Activator.CreateInstance<T>();

                //iterate though object properties
                foreach (var propertyInfo in @object.GetType().GetProperties())
                {
                    //get the map attribute
                    var attribute = ReflectionHelpers
                        .GetCustomAttributeFromProperty<MapAttribute>(propertyInfo);

                    //if the property is not mapped or the mapping value is wrong do nothing
                    if (attribute == null)
                    {
                        continue;
                    }

                    //set the value
                    try
                    {
                        //set the value of the property
                        propertyInfo.SetValue(@object, sqlDataReader[attribute.MapTo]);
                    }
                    catch (Exception)
                    {
                        //bad mapping let the property with default value
                    }
                }

                //return the object
                yield return @object;
            }
        }

        public static Task<IList<SqlParameter>> ConvertObjectPropertiesIntoSqlParametersUsing<T>(object @object) where T: MapAttribute
        {
            return Task.Run<IList<SqlParameter>>(() =>
            {
                var list = new List<SqlParameter>();

                foreach (var propertyInfo in @object?.GetType()?.GetProperties())
                {
                    //get the map attribute
                    if (!(propertyInfo.GetCustomAttribute(typeof(T)) is MapAttribute mapAttribute))
                    {
                        continue;
                    }

                    //ignore the key attribute
                    if (mapAttribute.IsKey)
                    {
                        continue;
                    }

                    //add the parameter into list
                    list.Add(
                        new SqlParameter($"@{mapAttribute.MapTo}", mapAttribute.Type)
                            {
                                Value = propertyInfo.GetValue(@object)
                            });
                }

                return list;
            });
        }   
    }
}
