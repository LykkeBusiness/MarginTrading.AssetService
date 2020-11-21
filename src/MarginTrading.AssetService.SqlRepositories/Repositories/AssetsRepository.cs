using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class AssetsRepository : IAssetsRepository
    {
        private readonly MsSqlContextFactory<AssetDbContext> _contextFactory;

        public AssetsRepository(MsSqlContextFactory<AssetDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyList<IAsset>> GetAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var products = await context.Products.Where(x => x.IsStarted).Select(x => new Asset(x.ProductId, x.Name, AssetConstants.Accuracy)).ToListAsync();
                var currencies = await context.Currencies.Select(x => new Asset(x.Id, x.Id, AssetConstants.Accuracy)).ToListAsync();

                return products.Union(currencies).ToList();
            }
        }

        public async Task<PaginatedResponse<IAsset>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            //Used raw SQL cause EF does not support Union after select from different tables yet
            skip ??= 0;
            take = PaginationHelper.GetTake(take);

            using (var context = _contextFactory.CreateDataContext())
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                var query = @$"SELECT ProductId as Id, Name as Name
                                    FROM [Products]
                                    WHERE IsStarted = '1'
                                    Union
                                    SELECT Id as Id, Id as Name
                                    FROM [Currencies]
                                    ORDER BY Name
                                    OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";

                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@skip", SqlDbType.Int) { Value = skip.Value });
                command.Parameters.Add(new SqlParameter("@take", SqlDbType.Int) { Value = take.Value });

                await context.Database.OpenConnectionAsync();

                var reader = await command.ExecuteReaderAsync();

                var assets = new List<IAsset>();
                while (await reader.ReadAsync())
                {
                    assets.Add(new Asset(reader["Id"].ToString(), reader["Name"].ToString(), AssetConstants.Accuracy));
                }

                await reader.CloseAsync();

                var totalCountQuery = "SELECT (SELECT COUNT(*) FROM [Products]) + (SELECT COUNT(*) FROM [Currencies])";
                command.CommandText = totalCountQuery;
                command.Parameters.Clear();

                var totalCount = (int)await command.ExecuteScalarAsync();

                return new PaginatedResponse<IAsset>(
                    contents: assets,
                    start: skip.Value,
                    size: assets.Count,
                    totalSize: totalCount
                );

            }
        }

        public async Task<IAsset> GetAsync(string assetId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.Products.Where(x => x.ProductId == assetId)
                                 .Select(x => new Asset(x.ProductId, x.Name, AssetConstants.Accuracy))
                                 .FirstOrDefaultAsync() ??
                             await context.Currencies.Where(x => x.Id == assetId)
                                 .Select(x => new Asset(x.Id, x.Id, AssetConstants.Accuracy))
                                 .FirstOrDefaultAsync();

                return result;
            }
        }
    }
}