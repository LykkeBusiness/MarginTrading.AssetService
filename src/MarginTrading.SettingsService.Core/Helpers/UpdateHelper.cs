// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
                if (value == null)
                    continue;

                var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                updateParams[propertyInfo.Name] = underlyingType?.IsEnum ?? false
                    ? Convert.ChangeType(value, underlyingType).ToString()
                    : value;
            }

            var result = new DynamicParameters();
            result.AddDynamicParams(updateParams);
            return result;
        }

        /// <summary>
        /// Builds an object for the Azure ReplaceAsync.
        /// T type must have string Id property.
        /// T type must contain the same reference type properties as TN.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="newObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TN"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T GetAzureReplaceObject<T, TN>(T current, TN newObject)
            where T : class
            where TN : class
        {
            var newObjTypeProps = typeof(TN).GetProperties().Where(x => !x.GetType().IsValueType).ToList();
            if (!newObjTypeProps.Any(x => x.Name == "Id" && x.PropertyType == typeof(string)))
            {
                throw new Exception($"Id property must reside in {newObject.GetType().Name} type.");
            }
            
            var currentObjTypeProps = typeof(T).GetProperties().Where(x => !x.GetType().IsValueType).ToList();
            if (newObjTypeProps.Any(t => !currentObjTypeProps.Select(x => x.Name).Contains(t.Name)))
            {
                throw new Exception($"{typeof(T)} type must contain the same reference type properties as {typeof(TN)}");
            }

            var result = Activator.CreateInstance<T>();
            foreach (var propertyInfo in newObjTypeProps)
            {
                propertyInfo.SetValue(result, propertyInfo.GetValue(newObject)
                                              ?? currentObjTypeProps.Single(x => x.Name == propertyInfo.Name)
                                                  .GetValue(current));
            }

            return result;
        }
    }
}