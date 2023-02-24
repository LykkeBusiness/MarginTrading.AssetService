// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging;
using Lykke.Snow.Common;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
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
                                                 "CONSTRAINT {0}_Id UNIQUE(Id)" +
                                                 ");";
        
        private static Type DataType => typeof(ITradingRoute);
        private static readonly string GetColumns = "[" + string.Join("],[", DataType.GetProperties().Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",",
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        private readonly ILogger<TradingRoutesRepository> _logger;
        
        public TradingRoutesRepository(IConvertService convertService, string connectionString, ILogger<TradingRoutesRepository> logger)
        {
            _convertService = convertService;
            _connectionString = connectionString;
            _logger = logger;

            using var conn = new SqlConnection(_connectionString);
            try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create {TableName} table", TableName);
                throw;
            }
        }

        public async Task<PaginatedResponse<ITradingRoute>> GetByPagesAsync(int? skip = null, int? take = null)
        {
            (skip, take) = PaginationUtils.ValidateSkipAndTake(skip, take);

            await using var conn = new SqlConnection(_connectionString);
            var paginationClause = $" ORDER BY [Oid] OFFSET {skip ?? 0} ROWS FETCH NEXT {take} ROWS ONLY";
            var gridReader = await conn.QueryMultipleAsync(
                $"SELECT * FROM {TableName} {paginationClause}; SELECT COUNT(*) FROM {TableName}");
            var tradingRoutes = (await gridReader.ReadAsync<TradingRouteEntity>()).ToList();
            var totalCount = await gridReader.ReadSingleAsync<int>();
                
            return new PaginatedResponse<ITradingRoute>(
                contents: tradingRoutes, 
                start: skip ?? 0, 
                size: tradingRoutes.Count, 
                totalSize: totalCount
            );
        }
        
        public async Task<IReadOnlyList<ITradingRoute>> GetAsync()
        {
            await using var conn = new SqlConnection(_connectionString);
            var objects = await conn.QueryAsync<TradingRouteEntity>($"SELECT * FROM {TableName}");
                
            return objects.Select(_convertService.Convert<TradingRouteEntity, TradingRoute>).ToList();
        }

        public async Task<ITradingRoute> GetAsync(string routeId)
        {
            await using var conn = new SqlConnection(_connectionString);
            var objects = await conn.QueryAsync<TradingRouteEntity>(
                $"SELECT * FROM {TableName} WHERE Id=@id", new {id = routeId});
                
            return objects.Select(_convertService.Convert<TradingRouteEntity, TradingRoute>).FirstOrDefault();
        }

        public async Task<bool> TryInsertAsync(ITradingRoute tradingRoute)
        {
            await using var conn = new SqlConnection(_connectionString);
            try
            {
                await conn.ExecuteAsync(
                    $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                    _convertService.Convert<ITradingRoute, TradingRouteEntity>(tradingRoute));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to insert a trading route with Id {TradingRouteId}", tradingRoute.Id);
                return false;
            }

            return true;
        }

        public async Task UpdateAsync(ITradingRoute tradingRoute)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                $"update {TableName} set {GetUpdateClause} where Id=@Id", 
                _convertService.Convert<ITradingRoute, TradingRouteEntity>(tradingRoute));
        }

        public async Task DeleteAsync(string routeId)
        {
            await using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                $"DELETE {TableName} WHERE Id=@Id", new { Id = routeId});
        }
    }
}