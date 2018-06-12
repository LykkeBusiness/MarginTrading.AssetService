using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Dapper;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.SqlRepositories.Entities;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;

namespace MarginTrading.SettingsService.SqlRepositories.Repositories
{
    public class TradingConditionsRepository : ITradingConditionsRepository
    {
        private const string TableName = "TradingConditions";
        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Oid] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[Id] [nvarchar] (64) NOT NULL, " +
                                                 "[Name] [nvarchar] (64) NOT NULL, " +
                                                 "[LegalEntity] [nvarchar] (64) NULL, " +
                                                 "[MarginCall1] [decimal] NULL, " +
                                                 "[MarginCall2] [decimal] NULL, " +
                                                 "[StopOut] [decimal] NULL, " +
                                                 "[DepositLimit] [decimal] NULL, " +
                                                 "[WithdrawalLimit] [decimal] NULL, " +
                                                 "[LimitCurrency] [nvarchar] (64) NULL, " +
                                                 "[BaseAssets] [nvarchar] (MAX) NULL, " +
                                                 "[IsDefault] [bit] NOT NULL " +
                                                 ");";
        
        private static Type DataType => typeof(ITradingCondition);
        private static readonly string GetColumns = string.Join(",", DataType.GetProperties().Select(x => x.Name));
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",",
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

        private readonly IConvertService _convertService;
        private readonly string _connectionString;
        private readonly ILog _log;
        
        public TradingConditionsRepository(IConvertService convertService, string connectionString, ILog log)
        {
            _convertService = convertService;
            _log = log;
            _connectionString = connectionString;
            
            using (var conn = new SqlConnection(_connectionString))
            {
                try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
                catch (Exception ex)
                {
                    _log?.WriteErrorAsync(nameof(TradingConditionsRepository), "CreateTableIfDoesntExists", null, ex);
                    throw;
                }
            }
        }

        public async Task<IReadOnlyList<ITradingCondition>> GetAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingConditionEntity>($"SELECT * FROM {TableName}");
                
                return objects.Select(_convertService.Convert<TradingConditionEntity, TradingCondition>).ToList();
            }
        }

        public async Task<ITradingCondition> GetAsync(string tradingConditionId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingConditionEntity>(
                    $"SELECT * FROM {TableName} WHERE Id=@id", new {id = tradingConditionId});
                
                return objects.Select(_convertService.Convert<TradingConditionEntity, TradingCondition>).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyList<ITradingCondition>> GetDefaultAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var objects = await conn.QueryAsync<TradingConditionEntity>(
                    $"SELECT * FROM {TableName} WHERE IsDefault = 1");
                
                return objects.Select(_convertService.Convert<TradingConditionEntity, TradingCondition>).ToList();
            }
        }

        public async Task<bool> TryInsertAsync(ITradingCondition tradingCondition)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})",
                        _convertService.Convert<ITradingCondition, TradingCondition>(tradingCondition));
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public async Task UpdateAsync(ITradingCondition tradingCondition)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause} where Id=@Id", 
                    _convertService.Convert<ITradingCondition, TradingCondition>(tradingCondition));
            }
        }
    }
}