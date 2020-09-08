using Lykke.Common.MsSql;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Extensions
{
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