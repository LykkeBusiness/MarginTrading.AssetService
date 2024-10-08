﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Snow.Common;

using MarginTrading.AssetService.Contracts.Products;
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

        public async Task<IReadOnlyList<string>> GetDuplicatedIsinsAsync(string[] isins)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var query = context.Products.Where(x => !x.IsDiscontinued);
                var longQuery = query.Select(x => x.IsinLong).Where(x => isins.Contains(x));
                var shortQuery = query.Select(x => x.IsinShort).Where(x => isins.Contains(x));

                var duplicates = await longQuery.Concat(shortQuery)
                    .ToListAsync();

                return duplicates;
            }
        }

        public async Task<IReadOnlyList<DiscontinuedProduct>> GetDiscontinuedProductsAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                return await context.Products
                    .Where(x => x.IsDiscontinued)
                    .Select(x => new DiscontinuedProduct
                    {
                        ProductId =  x.ProductId,
                        ActualDiscontinuedDate = x.ActualDiscontinuedDate
                    })
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyList<IAsset>> GetAsync()
        {
            var result = new Dictionary<string, IAsset>();
            using (var context = _contextFactory.CreateDataContext())
            {
                var products = await context.Products.Where(x => x.IsStarted).Select(x => new Asset(x.ProductId, x.Name, AssetConstants.Accuracy)).ToListAsync();
                var currencies = await context.Currencies.Select(x => new Asset(x.Id, x.Id, AssetConstants.Accuracy)).ToListAsync();

                foreach (var product in products)
                {
                    result.Add(product.Id, product);
                }

                foreach (var currency in currencies)
                {
                    if (!result.ContainsKey(currency.Id))
                    {
                        result.Add(currency.Id, currency);
                    }
                }

                return result.Values.ToList();
            }
        }

        public async Task<PaginatedResponse<IAsset>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            //Used raw SQL cause EF does not support Union after select from different tables yet
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

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