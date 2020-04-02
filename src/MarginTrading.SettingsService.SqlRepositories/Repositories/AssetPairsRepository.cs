// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common.Log;
using Dapper;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Helpers;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.SqlRepositories.Entities;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.SqlRepositories.Repositories
{
    public class AssetPairsRepository : IAssetPairsRepository
    {
        private const string TableName = "AssetPairs";

        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Oid] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[Id] [nvarchar] (64) NOT NULL, " +
                                                 "[Name] [nvarchar] (64) NOT NULL, " +
                                                 "[BaseAssetId] [nvarchar] (64) NULL, " +
                                                 "[QuoteAssetId] [nvarchar] (64) NULL, " +
                                                 "[Accuracy] [int] NOT NULL, " +
                                                 "[MarketId] [nvarchar] (64) NULL, " +
                                                 "[LegalEntity] [nvarchar] (64) NULL, " +
                                                 "[BasePairId] [nvarchar] (64) NULL, " +
                                                 "[MatchingEngineMode] [nvarchar] (64) NULL, " +
                                                 "[StpMultiplierMarkupBid] float NULL, " +
                                                 "[StpMultiplierMarkupAsk] float NULL, " +
                                                 "[IsSuspended] BIT NOT NULL DEFAULT 0, " +
                                                 "[IsFrozen] BIT NOT NULL DEFAULT 0, " +
                                                 "[IsDiscontinued] BIT NOT NULL DEFAULT 0, " +
                                                 "[FreezeInfo] [nvarchar] (255) NULL, " +
                                                 "CONSTRAINT {0}_Id UNIQUE(Id)," +
                                                 "CONSTRAINT IX_{0}_Unique UNIQUE (BaseAssetId, QuoteAssetId, LegalEntity)," +
                                                 "INDEX IX_{0}_Base (Id, Name, BaseAssetId, QuoteAssetId, LegalEntity, MatchingEngineMode)" +
                                                 ");";
        
        private static PropertyInfo[] TypeProps => typeof(IAssetPair).GetProperties()
            .Where(x => x.Name != nameof(IAssetPair.IsSuspended)).ToArray();//get rid of Suspended flag, it is handled separately
        private static readonly string GetColumns = "[" + string.Join("],[", TypeProps.Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", TypeProps.Select(x => "@" + x.Name));
        private static readonly Func<IEnumerable<string>, string> GetUpdateClause = (ps) =>
            string.Join(",", ps.Select(x => "[" + x + "]=@" + x));

        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        private readonly ILog _log;
        
        public AssetPairsRepository(IConvertService convertService, string connectionString, ILog log)
        {
            _convertService = convertService;
            _log = log;
            _connectionString = connectionString;
            
            using (var conn = new SqlConnection(_connectionString))
            {
                try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
                catch (Exception ex)
                {
                    _log?.WriteErrorAsync(nameof(AssetPairsRepository), "CreateTableIfDoesntExists", null, ex);
                    throw;
                }
            }
        }

        public async Task<IAssetPair> GetByBaseQuoteAndLegalEntityAsync(string baseAssetId, string quoteAssetId, 
            string legalEntity)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetPairEntity>(
                    $"SELECT * FROM {TableName} WHERE BaseAssetId=@baseAssetId AND QuoteAssetId=@quoteAssetId AND LegalEntity=@legalEntity",
                    new {baseAssetId, quoteAssetId, legalEntity});
                
                return objects.FirstOrDefault();
            }
        }

        public async Task<IAssetPair> InsertAsync(IAssetPair obj)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        _convertService.Convert<IAssetPair, AssetPairEntity>(obj));

                    return await conn.QuerySingleOrDefaultAsync<AssetPairEntity>(
                        $"SELECT * FROM {TableName} WHERE Id=@id", new {obj.Id});
                }
                catch (Exception ex)
                {
                    _log?.WriteWarningAsync(nameof(AssetPairsRepository), nameof(InsertAsync),
                        $"Failed to insert an asset pair with Id {obj.Id}", ex);
                    return null;
                }
            }
        }

        public async Task<IReadOnlyList<IAssetPair>> InsertBatchAsync(IReadOnlyList<IAssetPair> assetPairs)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                SqlTransaction transaction = null;
                
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                    }
                    
                    transaction = conn.BeginTransaction();

                    if (await conn.ExecuteScalarAsync<int>(
                            $"SELECT COUNT(*) FROM {TableName} WITH (UPDLOCK) WHERE Id IN ({string.Join(",", assetPairs.Select(x => $"'{x.Id}'"))})",
                            new { },
                            transaction) > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(assetPairs), "One of asset pairs already exist");
                    }

                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        assetPairs.Select(_convertService.Convert<IAssetPair, AssetPairEntity>),
                        transaction);

                    var inserted = await conn.QueryAsync<AssetPairEntity>(
                        $"SELECT * FROM {TableName} WITH (UPDLOCK) WHERE Id IN ({string.Join(",", assetPairs.Select(x => $"'{x.Id}'"))})",
                        new {},
                        transaction);
                    
                    transaction.Commit();
                    
                    return inserted.ToList();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    await _log.WriteErrorAsync(nameof(AssetPairsRepository),
                        nameof(InsertBatchAsync), $"Failed to perform batch transaction: {ex.Message}", ex);
                    
                    return null;
                }
            }
        }

        public async Task<IAssetPair> UpdateAsync(AssetPairUpdateRequest assetPairUpdateRequest)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var updateObj = UpdateHelper.GetSqlUpdateObject(assetPairUpdateRequest);
                
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause(updateObj.ParameterNames)} where Id=@Id", 
                    updateObj);

                return await conn.QuerySingleOrDefaultAsync<AssetPairEntity>(
                    $"SELECT * FROM {TableName} WHERE Id=@id", new {assetPairUpdateRequest.Id});
            }
        }

        public async Task<IReadOnlyList<IAssetPair>> UpdateBatchAsync(
            IReadOnlyList<AssetPairUpdateRequest> assetPairsUpdateRequest)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                SqlTransaction transaction = null;
                
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        await conn.OpenAsync();
                    }
                    
                    transaction = conn.BeginTransaction();

                    if (await conn.ExecuteScalarAsync<int>(
                            $"SELECT COUNT(*) FROM {TableName} WITH (UPDLOCK) WHERE Id IN ({string.Join(",", assetPairsUpdateRequest.Select(x => $"'{x.Id}'"))})",
                            new { },
                            transaction) != assetPairsUpdateRequest.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(assetPairsUpdateRequest), "One of asset pairs does not exist");
                    }

                    foreach (var assetPair in assetPairsUpdateRequest)
                    {   
                        var updateObj = UpdateHelper.GetSqlUpdateObject(assetPair);
                        await conn.ExecuteAsync(
                            $"update {TableName} set {GetUpdateClause(updateObj.ParameterNames)} where Id=@Id", 
                            updateObj,
                            transaction);
                    }

                    var updated = await conn.QueryAsync<AssetPairEntity>(
                        $"SELECT * FROM {TableName} WITH (UPDLOCK) WHERE Id IN ({string.Join(",", assetPairsUpdateRequest.Select(x => $"'{x.Id}'"))})",
                        new {},
                        transaction);

                    transaction.Commit();
                    
                    return updated.ToList();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    await _log.WriteErrorAsync(nameof(AssetPairsRepository),
                        nameof(InsertBatchAsync), $"Failed to perform batch transaction: {ex.Message}", ex);
                    
                    return null;
                }
            }
        }

        public async Task<IAssetPair> ChangeSuspendFlag(string assetPairId, bool suspendFlag)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set IsSuspended = @IsSuspended where Id=@Id", 
                    new
                    {
                        Id = assetPairId,
                        IsSuspended = suspendFlag,
                    });
            }
            //todo may be optimized with QueryMultipleAsync
            return await GetAsync(assetPairId);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"DELETE {TableName} WHERE Id=@Id", new { Id = assetPairId});
            }
        }

        public async Task<IReadOnlyList<IAssetPair>> GetAsync(params string[] assetPairIds)
        {
            assetPairIds = assetPairIds ?? new string[0];
            
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = assetPairIds.Length == 0
                    ? $"SELECT * FROM {TableName}"
                    : $"SELECT * FROM {TableName} WHERE Id IN @assetPairIds";
                var objects = await conn.QueryAsync<AssetPairEntity>(query, new {assetPairIds});
                
                return objects.Select(_convertService.Convert<AssetPairEntity, AssetPair>).ToList();
            }
        }

        public async Task<IAssetPair> GetAsync(string assetPairId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetPairEntity>(
                    $"SELECT * FROM {TableName} WHERE Id=@id", new {id = assetPairId});
                
                return objects.Select(_convertService.Convert<AssetPairEntity, AssetPair>).FirstOrDefault();
            }
        }

        public async Task<IAssetPair> GetByBaseAssetPairAsync(string baseAssetPairId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetPairEntity>(
                    $"SELECT * FROM {TableName} WHERE BaseAssetId=@baseAssetPairId", new {baseAssetPairId});
                
                return objects.Select(_convertService.Convert<AssetPairEntity, AssetPair>).FirstOrDefault();
            }
        }

        public async Task<IAssetPair> GetByBaseAssetPairAndNotByIdAsync(string id, string baseAssetPairId)
        {
            const string whereClause = "WHERE 1=1 "
                                       + " AND Id<>@id"
                                       + " AND BaseAssetId=@baseAssetPairId";

            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetPairEntity>($"SELECT * FROM {TableName} {whereClause}",
                    new {id, baseAssetPairId});
                
                return objects.Select(_convertService.Convert<AssetPairEntity, AssetPair>).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyList<IAssetPair>> GetByLeAndMeModeAsync(string legalEntity = null,
            string matchingEngineMode = null, string filter = null)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = $"%{filter}%";
            }
            
            var whereClause = "WHERE 1=1 "
                + (string.IsNullOrWhiteSpace(legalEntity) ? "" : " AND LegalEntity=@legalEntity")
                + (string.IsNullOrWhiteSpace(matchingEngineMode) ? "" : " AND MatchingEngineMode=@matchingEngineMode")
                + (string.IsNullOrWhiteSpace(filter) ? "" : " AND (Id LIKE @filter OR Name LIKE @filter)");
            
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetPairEntity>($"SELECT * FROM {TableName} {whereClause}",
                    new {legalEntity, matchingEngineMode, filter});
                
                return objects.Select(_convertService.Convert<AssetPairEntity, AssetPair>).ToList();
            }
        }

        public async Task<PaginatedResponse<IAssetPair>> GetByLeAndMeModeByPagesAsync(string legalEntity = null,
            string matchingEngineMode = null, string filter = null, int? skip = null, int? take = null)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = $"%{filter}%";
            }

            var whereClause = "WHERE 1=1 "
                              + (string.IsNullOrWhiteSpace(legalEntity) ? "" : " AND LegalEntity=@legalEntity")
                              + (string.IsNullOrWhiteSpace(matchingEngineMode) ? "" : " AND MatchingEngineMode=@matchingEngineMode")
                              + (string.IsNullOrWhiteSpace(filter) ? "" : " AND (Id LIKE @filter OR Name LIKE @filter)");
            
            using (var conn = new SqlConnection(_connectionString))
            {
                var paginationClause = $" ORDER BY [Oid] OFFSET {skip ?? 0} ROWS FETCH NEXT {PaginationHelper.GetTake(take)} ROWS ONLY";
                var gridReader = await conn.QueryMultipleAsync(
                    $"SELECT * FROM {TableName} {whereClause} {paginationClause}; SELECT COUNT(*) FROM {TableName} {whereClause}",
                    new {legalEntity, matchingEngineMode, filter});
                var assetPairs = (await gridReader.ReadAsync<AssetPairEntity>()).ToList();
                var totalCount = await gridReader.ReadSingleAsync<int>();
            
                return new PaginatedResponse<IAssetPair>(
                    contents: assetPairs, 
                    start: skip ?? 0, 
                    size: assetPairs.Count, 
                    totalSize: totalCount
                );
            }
        }
    }
}