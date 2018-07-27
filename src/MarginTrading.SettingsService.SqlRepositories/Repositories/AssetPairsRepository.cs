using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Dapper;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
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
                                                 "CONSTRAINT AP_Id UNIQUE(Id)" +
                                                 ");";
        
        private static Type DataType => typeof(IAssetPair);
        private static readonly string GetColumns = "[" + string.Join("],[", DataType.GetProperties().Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",",
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

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

        public async Task<bool> TryInsertAsync(IAssetPair obj)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        _convertService.Convert<IAssetPair, AssetPairEntity>(obj));
                }
                catch (Exception ex)
                {
                    _log?.WriteWarningAsync(nameof(AssetPairsRepository), nameof(TryInsertAsync),
                        $"Failed to insert an asset pair with Id {obj.Id}", ex);
                    return false;
                }

                return true;
            }
        }

        public async Task UpdateAsync(IAssetPair obj)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause} where Id=@Id", 
                    _convertService.Convert<IAssetPair, AssetPairEntity>(obj));
            }
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
            string matchingEngineMode = null)
        {
            var whereClause = "WHERE 1=1 "
                + (string.IsNullOrWhiteSpace(legalEntity) ? "" : " AND LegalEntity=@legalEntity")
                + (string.IsNullOrWhiteSpace(matchingEngineMode) ? "" : " AND MatchingEngineMode=@matchingEngineMode");
            
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetPairEntity>($"SELECT * FROM {TableName} {whereClause}",
                    new {legalEntity, matchingEngineMode});
                
                return objects.Select(_convertService.Convert<AssetPairEntity, AssetPair>).ToList();
            }
        }

        public async Task<PaginatedResponse<IAssetPair>> GetByLeAndMeModeByPagesAsync(string legalEntity = null, 
            string matchingEngineMode = null, int? skip = null, int? take = null)
        {
            var whereClause = "WHERE 1=1 "
                              + (string.IsNullOrWhiteSpace(legalEntity) ? "" : " AND LegalEntity=@legalEntity")
                              + (string.IsNullOrWhiteSpace(matchingEngineMode) ? "" : " AND MatchingEngineMode=@matchingEngineMode");
            
            using (var conn = new SqlConnection(_connectionString))
            {
                var paginationClause = $" ORDER BY [Oid] OFFSET {skip ?? 0} ROWS FETCH NEXT {PaginationHelper.GetTake(take)} ROWS ONLY";
                var gridReader = await conn.QueryMultipleAsync(
                    $"SELECT * FROM {TableName} {whereClause} {paginationClause}; SELECT COUNT(*) FROM {TableName} {whereClause}",
                    new {legalEntity, matchingEngineMode});
                var assetPairs = (await gridReader.ReadAsync<AssetPairEntity>()).ToList();
                var totalCount = await gridReader.ReadSingleAsync<int>();
            
                return new PaginatedResponse<IAssetPair>(
                    contents: assetPairs, 
                    start: skip ?? 0, 
                    size: assetPairs.Count, 
                    totalSize: !take.HasValue ? assetPairs.Count : totalCount
                );
            }
        }
    }
}