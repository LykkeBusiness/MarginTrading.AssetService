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
    public class AssetsRepository: IAssetsRepository
    {
        private const string TableName = "Assets";
        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Oid] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[Id] [nvarchar] (64) NOT NULL, " +
                                                 "[Name] [nvarchar] (64) NOT NULL, " +
                                                 "[Accuracy] [int] NOT NULL, " +
                                                 "CONSTRAINT A_Id UNIQUE(Id)" +
                                                 ");";
        
        private static Type DataType => typeof(IAsset);
        private static readonly string GetColumns = "[" + string.Join("],[", DataType.GetProperties().Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",",
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        private readonly ILog _log;
        
        public AssetsRepository(IConvertService convertService, string connectionString, ILog log)
        {
            _convertService = convertService;
            _log = log;
            _connectionString = connectionString;
            
            using (var conn = new SqlConnection(_connectionString))
            {
                try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
                catch (Exception ex)
                {
                    _log?.WriteErrorAsync(nameof(AssetsRepository), "CreateTableIfDoesntExists", null, ex);
                    throw;
                }
            }
        }

        public async Task<IReadOnlyList<IAsset>> GetAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetEntity>($"SELECT * FROM {TableName}");
                
                return objects.Select(_convertService.Convert<AssetEntity, Asset>).ToList();
            }
        }

        public async Task<PaginatedResponse<IAsset>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var paginationClause = $" ORDER BY [Oid] OFFSET {skip ?? 0} ROWS FETCH NEXT {PaginationHelper.GetTake(take)} ROWS ONLY";
                var gridReader = await conn.QueryMultipleAsync(
                    $"SELECT * FROM {TableName} {paginationClause}; SELECT COUNT(*) FROM {TableName}");
                var assets = (await gridReader.ReadAsync<AssetEntity>()).ToList();
                var totalCount = await gridReader.ReadSingleAsync<int>();
            
                return new PaginatedResponse<IAsset>(
                    contents: assets, 
                    start: skip ?? 0, 
                    size: assets.Count, 
                    totalSize: !take.HasValue ? assets.Count : totalCount
                );
            }
        }

        public async Task<IAsset> GetAsync(string assetId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<AssetEntity>(
                    $"SELECT * FROM {TableName} WHERE Id=@id", new {id = assetId});
                
                return objects.Select(_convertService.Convert<AssetEntity, Asset>).FirstOrDefault();
            }
        }

        public async Task<bool> TryInsertAsync(IAsset asset)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        _convertService.Convert<IAsset, AssetEntity>(asset));
                }
                catch (Exception ex)
                {
                    _log?.WriteWarningAsync(nameof(AssetPairsRepository), nameof(TryInsertAsync),
                        $"Failed to insert an asset with Id {asset.Id}", ex);
                    return false;
                }

                return true;
            }
        }

        public async Task UpdateAsync(IAsset asset)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause} where Id=@Id", 
                    _convertService.Convert<IAsset, AssetEntity>(asset));
            }
        }

        public async Task DeleteAsync(string assetId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"DELETE {TableName} WHERE Id=@Id", new { Id = assetId});
            }
        }
    }
}