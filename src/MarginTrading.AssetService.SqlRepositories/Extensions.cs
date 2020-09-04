// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Lykke.Common.MsSql;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories
{
    public static class Extensions
    {
        public static void CreateTableIfDoesntExists(this IDbConnection connection, string createQuery,
            string tableName)
        {
            connection.Open();
            try
            {
                // Check if table exists
                connection.ExecuteScalar($"select top 1 * from {tableName}");
            }
            catch (SqlException)
            {
                // Create table
                var query = string.Format(createQuery, tableName);
                connection.Query(query);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    
    public static class DbUpdateExceptionExtensions
    {
        public static bool ValueAlreadyExistsException(this DbUpdateException e)
        {
            return e.InnerException is SqlException sqlException &&
                   (sqlException.Number == MsSqlErrorCodes.PrimaryKeyConstraintViolation ||
                    sqlException.Number == MsSqlErrorCodes.DuplicateIndex);
        }
    }
}