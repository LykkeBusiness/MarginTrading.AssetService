using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Dapper;

namespace MarginTrading.SettingsService.Core.Helpers
{
    public static class UpdateHelper
    {
        /// <summary>
        /// Builds an object for a customized SQL update. 
        /// </summary>
        /// <param name="newObject"></param>
        /// <typeparam name="T">T type must have string Id property</typeparam>
        /// <returns>Dynamic object with non-null properties of <param name="newObject"/>. Null properties are skipped.</returns>
        /// <exception cref="Exception"></exception>
        public static DynamicParameters GetSqlUpdateObject<T>(T newObject)
            where T : class
        {
            var updateParams = new Dictionary<string, object>();
            
            var refTypeProps = newObject.GetType().GetProperties().Where(x => !x.GetType().IsValueType).ToList();
            if (!refTypeProps.Any(x => x.Name == "Id" && x.PropertyType == typeof(string)))
            {
                throw new Exception($"Id property must reside in {newObject.GetType().Name} type.");
            }
            
            foreach (var propertyInfo in refTypeProps)
            {
                var value = propertyInfo.GetValue(newObject);
                if (value != null)
                {
                    updateParams[propertyInfo.Name] = value;
                }
            }

            var result = new DynamicParameters();
            result.AddDynamicParams(updateParams);
            return result;
        }

        /// <summary>
        /// Builds an object for an Azure ReplaceAsync. 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="newObject"></param>
        /// <typeparam name="T">T type must have string Id property</typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T GetAzureReplaceObject<T>(T current, T newObject)
            where T : class
        {
            var refTypeProps = newObject.GetType().GetProperties().Where(x => !x.GetType().IsValueType).ToList();
            if (!refTypeProps.Any(x => x.Name == "Id" && x.PropertyType == typeof(string)))
            {
                throw new Exception($"Id property must reside in {newObject.GetType().Name} type.");
            }

            var result = Activator.CreateInstance<T>();
            foreach (var propertyInfo in refTypeProps)
            {
                propertyInfo.SetValue(result, 
                    propertyInfo.GetValue(newObject) ?? propertyInfo.GetValue(current));
            }
            
            return result;
        }
    }
}