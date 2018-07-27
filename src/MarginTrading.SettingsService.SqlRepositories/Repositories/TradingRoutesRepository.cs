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
    public class TradingRoutesRepository : ITradingRoutesRepository
    {
        private const string TableName = "TradingRoutes";
        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Oid] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[Id] [nvarchar] (64) NOT NULL, " +
                                                 "[Rank] [int] NULL, " +
                                                 "[TradingConditionId] [nvarchar] (64) NULL, " +
                                                 "[ClientId] [nvarchar] (64) NULL, " +
                                                 "[Instrument] [nvarchar] (64) NULL, " +
                                                 "[Type] [nvarchar] (64) NULL, " +
                                                 "[MatchingEngineId] [nvarchar] (64) NULL, " +
                                                 "[Asset] [nvarchar] (64) NULL, " +
                                                 "[RiskSystemLimitType] [nvarchar] (64) NULL, " +
                                                 "[RiskSystemMetricType] [nvarchar] (64) NULL, " +
                                                 "CONSTRAINT TR_Id UNIQUE(Id)" +
                                                 ");";
        
        private static Type DataType => typeof(ITradingRoute);
        private static readonly string GetColumns = "[" + string.Join("],[", DataType.GetProperties().Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",",
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        private readonly ILog _log;
        
        public TradingRoutesRepository(IConvertService convertService, string connectionString, ILog log)
        {
            _convertService = convertService;
            _log = log;
            _connectionString = connectionString;
            
            using (var conn = new SqlConnection(_connectionString))
            {
                try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
                catch (Exception ex)
                {
                    _log?.WriteErrorAsync(nameof(TradingRoutesRepository), "CreateTableIfDoesntExists", null, ex);
                    throw;
                }
            }
        }

        public async Task<IReadOnlyList<ITradingRoute>> GetAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingRouteEntity>($"SELECT * FROM {TableName}");
                
                return objects.Select(_convertService.Convert<TradingRouteEntity, TradingRoute>).ToList();
            }
        }

        public async Task<PaginatedResponse<ITradingRoute>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var paginationClause = $" ORDER BY [Oid] OFFSET {skip ?? 0} ROWS FETCH NEXT {PaginationHelper.GetTake(take)} ROWS ONLY";
                var gridReader = await conn.QueryMultipleAsync(
                    $"SELECT * FROM {TableName} {paginationClause}; SELECT COUNT(*) FROM {TableName}");
                var tradingRoutes = (await gridReader.ReadAsync<TradingRouteEntity>()).ToList();
                var totalCount = await gridReader.ReadSingleAsync<int>();
                
                return new PaginatedResponse<ITradingRoute>(
                    contents: tradingRoutes, 
                    start: skip ?? 0, 
                    size: tradingRoutes.Count, 
                    totalSize: !take.HasValue ? tradingRoutes.Count : totalCount
                );
            }
        }

        public async Task<ITradingRoute> GetAsync(string routeId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingRouteEntity>(
                    $"SELECT * FROM {TableName} WHERE Id=@id", new {id = routeId});
                
                return objects.Select(_convertService.Convert<TradingRouteEntity, TradingRoute>).FirstOrDefault();
            }
        }

        public async Task<bool> TryInsertAsync(ITradingRoute tradingRoute)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        _convertService.Convert<ITradingRoute, TradingRouteEntity>(tradingRoute));
                }
                catch (Exception ex)
                {
                    _log?.WriteWarningAsync(nameof(AssetPairsRepository), nameof(TryInsertAsync),
                        $"Failed to insert a trading route with Id {tradingRoute.Id}", ex);
                    return false;
                }

                return true;
            }
        }

        public async Task UpdateAsync(ITradingRoute tradingRoute)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause} where Id=@Id", 
                    _convertService.Convert<ITradingRoute, TradingRouteEntity>(tradingRoute));
            }
        }

        public async Task DeleteAsync(string routeId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"DELETE {TableName} WHERE Id=@Id", new { Id = routeId});
            }
        }
    }
}