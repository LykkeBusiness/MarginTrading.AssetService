// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

using Microsoft.Data.SqlClient;

namespace MarginTrading.AssetService.SqlRepositories.Extensions
{
    // todo: consider moving to NuGet package 
    internal static class SqlServerParametersExtensions
    {
        /// <summary>
        /// Maximum number of parameters allowed in a single query by SQL Server.
        /// </summary>
        public const int MaxParametersCount = 2100;
        
        /// <summary>
        /// Builds an array of SqlParameters from an array of objects.
        /// Parameters indexes are zero-based if no start index is provided.
        /// </summary>
        /// <param name="source">The array of objects to map to SqlParameters</param>
        /// <param name="prefix">The prefix to use for the parameter names</param>
        /// <param name="start">The index to start the parameter names at</param>
        /// <typeparam name="T">The type of the objects in the array. Must implement ToString()</typeparam>
        /// <returns></returns>
        public static SqlParameter[] MapToParameters<T>(this T[] source, string prefix = "@p", int start = 0)
        {
            if (source == null || !source.Any())
                return Array.Empty<SqlParameter>();

            var result = source.Select((x, i) => new SqlParameter($"{prefix}{i + start}", x.ToString()));
            
            return result.ToArray();
        }
        
        /// <summary>
        /// Builds a string of comma-separated parameter names from an array of SqlParameters.
        /// </summary>
        /// <param name="source">The array of SqlParameters to build the string from</param>
        /// <returns></returns>
        public static string ToSqlInClause(this SqlParameter[] source)
        {
            if (source == null || !source.Any())
                return string.Empty;

            var result = source.Select(x => x.ParameterName);
            
            return string.Join(",", result);
        }
    }
}