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
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.SqlRepositories.Entities;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.SqlRepositories.Repositories
{
    public class TradingInstrumentsRepository : ITradingInstrumentsRepository
    {
        private const string TableName = "TradingInstruments";
        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Oid] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[TradingConditionId] [nvarchar] (64) NOT NULL, " +
                                                 "[Instrument] [nvarchar] (64) NOT NULL, " +
                                                 "[LeverageInit] [int] NULL, " +
                                                 "[LeverageMaintenance] [int] NULL, " +
                                                 "[SwapLong] float NULL, " +
                                                 "[SwapShort] float NULL, " +
                                                 "[Delta] float NULL, " +
                                                 "[DealMinLimit] float NULL, " +
                                                 "[DealMaxLimit] float NULL, " +
                                                 "[PositionLimit] float NULL, " +
                                                 "[CommissionRate] float NULL, " +
                                                 "[CommissionMin] float NULL, " +
                                                 "[CommissionMax] float NULL, " +
                                                 "[CommissionCurrency] [nvarchar] (64) NULL " +
                                                 ");";
        
        private static Type DataType => typeof(ITradingInstrument);
        private static readonly string GetColumns = "[" + string.Join("],[", DataType.GetProperties().Select(x => x.Name)) + "]";
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",",
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        private readonly ILog _log;
        
        public TradingInstrumentsRepository(IConvertService convertService, string connectionString, ILog log)
        {
            _convertService = convertService;
            _log = log;
            _connectionString = connectionString;
            
            using (var conn = new SqlConnection(_connectionString))
            {
                try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
                catch (Exception ex)
                {
                    _log?.WriteErrorAsync(nameof(TradingInstrumentsRepository), "CreateTableIfDoesntExists", null, ex);
                    throw;
                }
            }
        }

        public async Task<IReadOnlyList<ITradingInstrument>> GetAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingInstrumentEntity>(
                    $"SELECT * FROM {TableName}");
                
                return objects.Select(_convertService.Convert<TradingInstrumentEntity, TradingInstrument>).ToList();
            }
        }

        public async Task<IReadOnlyList<ITradingInstrument>> GetByTradingConditionAsync(string tradingConditionId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingInstrumentEntity>(
                    $"SELECT * FROM {TableName} WHERE TradingConditionId=@tradingConditionId",
                    new {tradingConditionId});
                
                return objects.Select(_convertService.Convert<TradingInstrumentEntity, TradingInstrument>).ToList();
            }
        }

        public async Task<PaginatedResponse<ITradingInstrument>> GetByPagesAsync(string tradingConditionId = null, 
            int? skip = null, int? take = null)
        {
            var whereClause = "WHERE 1=1 "
                              + (string.IsNullOrWhiteSpace(tradingConditionId) ? "" : " AND TradingConditionId=@tradingConditionId");
            
            using (var conn = new SqlConnection(_connectionString))
            {
                var paginationClause = $" ORDER BY [Oid] OFFSET {skip ?? 0} ROWS FETCH NEXT {PaginationHelper.GetTake(take)} ROWS ONLY";
                var gridReader = await conn.QueryMultipleAsync(
                    $"SELECT * FROM {TableName} {whereClause} {paginationClause}; SELECT COUNT(*) FROM {TableName} {whereClause}",
                    new {tradingConditionId});
                var tradingInstruments = (await gridReader.ReadAsync<TradingInstrumentEntity>()).ToList();
                var totalCount = await gridReader.ReadSingleAsync<int>();
             
                return new PaginatedResponse<ITradingInstrument>(
                    contents: tradingInstruments, 
                    start: skip ?? 0, 
                    size: tradingInstruments.Count, 
                    totalSize: !take.HasValue ? tradingInstruments.Count : totalCount
                );
            }
        }

        public async Task<ITradingInstrument> GetAsync(string assetPairId, string tradingConditionId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingInstrumentEntity>(
                    $"SELECT * FROM {TableName} WHERE TradingConditionId=@tradingConditionId AND Instrument=@assetPairId",
                    new {tradingConditionId, assetPairId});
                
                return objects.Select(_convertService.Convert<TradingInstrumentEntity, TradingInstrument>).FirstOrDefault();
            }
        }

        public async Task<bool> TryInsertAsync(ITradingInstrument tradingInstrument)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    if (null != await conn.QueryFirstOrDefaultAsync<TradingInstrumentEntity>(
                            $"SELECT * FROM {TableName} WHERE TradingConditionId=@tradingConditionId AND Instrument=@assetPairId",
                            new
                            {
                                tradingConditionId = tradingInstrument.TradingConditionId, 
                                assetPairId = tradingInstrument.Instrument
                            }))
                    {
                        return false;
                    }

                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        _convertService.Convert<ITradingInstrument, TradingInstrumentEntity>(tradingInstrument));
                }
                catch (Exception ex)
                {
                    _log?.WriteWarningAsync(nameof(AssetPairsRepository), nameof(TryInsertAsync),
                        $"Failed to insert a trading instrument with assetPairId {tradingInstrument.Instrument} and tradingConditionId {tradingInstrument.TradingConditionId}", ex);
                    return false;
                }

                return true;
            }
        }

        public async Task UpdateAsync(ITradingInstrument tradingInstrument)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause} where TradingConditionId=@TradingConditionId AND Instrument=@Instrument", 
                    _convertService.Convert<ITradingInstrument, TradingInstrumentEntity>(tradingInstrument));
            }
        }

        public async Task DeleteAsync(string assetPairId, string tradingConditionId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"DELETE {TableName} WHERE TradingConditionId=@tradingConditionId AND Instrument=@assetPairId",
                    new
                    {
                        tradingConditionId,
                        assetPairId
                    });
            }
        }

        public async Task<IEnumerable<ITradingInstrument>> CreateDefaultTradingInstruments(string tradingConditionId, 
            IEnumerable<string> assetPairsIds, DefaultTradingInstrumentSettings defaults)
        {
            var objectsToAdd = assetPairsIds.Select(x => new TradingInstrument
            (
                tradingConditionId,
                x,
                defaults.LeverageInit,
                defaults.LeverageMaintenance,
                defaults.SwapLong,
                defaults.SwapShort,
                defaults.Delta,
                defaults.DealMinLimit,
                defaults.DealMaxLimit,
                defaults.PositionLimit,
                defaults.CommissionRate,
                defaults.CommissionMin,
                defaults.CommissionMax,
                defaults.CommissionCurrency
            )).ToList();
            
            
                using (var conn = new SqlConnection(_connectionString))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        transaction = conn.BeginTransaction();
                        
                        await conn.ExecuteAsync(
                            $"insert into {TableName} ({GetColumns}) values ({GetFields})", 
                            objectsToAdd.Select(_convertService.Convert<TradingInstrument, TradingInstrumentEntity>), 
                            transaction);
                        
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction?.Rollback();
                        await _log.WriteErrorAsync(nameof(TradingInstrumentsRepository),
                            nameof(CreateDefaultTradingInstruments), "Failed to create default trading instruments", ex);
                    }
                }
           

            return objectsToAdd;
        }
    }
}