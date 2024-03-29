﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Dapper;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.SqlRepositories.Entities;
using MarginTrading.AssetService.SqlRepositories.Extensions;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarginTrading.AssetService.SqlRepositories.Repositories
{
    public class OperationExecutionInfoRepository : IOperationExecutionInfoRepository
    {
        private const string TableName = "SettingsServiceExecutionInfo";
        private const string CreateTableScript = "CREATE TABLE [{0}](" +
                                                 "[Oid] [bigint] NOT NULL IDENTITY(1,1) PRIMARY KEY," +
                                                 "[Id] [nvarchar] (64) NOT NULL," +
                                                 "[LastModified] [datetime] NOT NULL, " +
                                                 "[OperationName] [nvarchar] (64) NULL, " +
                                                 "[Version] [nvarchar] (64) NULL, " +
                                                 "[Data] [nvarchar] (MAX) NOT NULL," +
                                                 "CONSTRAINT [{0}_Id] UNIQUE NONCLUSTERED ([Id], [OperationName])" +
                                                 ");";
        
        private static Type DataType => typeof(IOperationExecutionInfo<object>);
        private static readonly string GetColumns = string.Join(",", DataType.GetProperties().Select(x => x.Name));
        private static readonly string GetFields = string.Join(",", DataType.GetProperties().Select(x => "@" + x.Name));
        private static readonly string GetUpdateClause = string.Join(",", 
            DataType.GetProperties().Select(x => "[" + x.Name + "]=@" + x.Name));

        private readonly string _connectionString;
        private readonly ILogger<OperationExecutionInfoRepository> _logger;
        private readonly ISystemClock _systemClock;

        public OperationExecutionInfoRepository( 
            string connectionString, 
            ISystemClock systemClock,
            ILogger<OperationExecutionInfoRepository> logger)
        {
            _connectionString = connectionString;
            _systemClock = systemClock;
            _logger = logger;

            using var conn = new SqlConnection(_connectionString);
            try { conn.CreateTableIfDoesntExists(CreateTableScript, TableName); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create table {TableName}", TableName);
                throw;
            }
        }
        
        public async Task<IOperationExecutionInfo<TData>> GetOrAddAsync<TData>(
            string operationName, string operationId, Func<IOperationExecutionInfo<TData>> factory) where TData : class
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                var operationInfo = await conn.QueryFirstOrDefaultAsync<OperationExecutionInfoEntity>(
                    $"SELECT * FROM {TableName} WHERE Id=@operationId and OperationName=@operationName",
                    new {operationId, operationName});

                if (operationInfo == null)
                {
                    var entity = Convert(factory());
                    entity.LastModified = _systemClock.UtcNow.UtcDateTime;

                    await conn.ExecuteAsync(
                        $"insert into {TableName} ({GetColumns}) values ({GetFields})", entity);

                    return Convert<TData>(entity);
                }

                return Convert<TData>(operationInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get or add operation execution info for {OperationName} {OperationId}",
                    operationName, operationId);
                throw;
            }
        }

        public async Task<IOperationExecutionInfo<TData>> GetAsync<TData>(string operationName, string id) where TData : class
        {
            await using var conn = new SqlConnection(_connectionString);
            var operationInfo = await conn.QuerySingleOrDefaultAsync<OperationExecutionInfoEntity>(
                $"SELECT * FROM {TableName} WHERE Id = @id and OperationName=@operationName",
                new {id, operationName});

            return operationInfo == null ? null : Convert<TData>(operationInfo);
        }

        public async Task Save<TData>(IOperationExecutionInfo<TData> executionInfo) where TData : class
        {
            var entity = Convert(executionInfo);
            entity.LastModified = _systemClock.UtcNow.UtcDateTime;

            await using var conn = new SqlConnection(_connectionString);
            try
            {
                await conn.ExecuteAsync(
                    $"insert into {TableName} ({GetColumns}) values ({GetFields})", entity);
            }
            catch (SqlException)
            {
                await conn.ExecuteAsync(
                    $"update {TableName} set {GetUpdateClause} where Id=@Id and OperationName=@OperationName", entity);
            }
        }
        
        private static OperationExecutionInfo<TData> Convert<TData>(OperationExecutionInfoEntity entity)
            where TData : class
        {
            return new OperationExecutionInfo<TData>(
                operationName: entity.OperationName,
                id: entity.Id,
                lastModified: entity.LastModified,
                data: entity.Data is string dataStr
                    ? JsonConvert.DeserializeObject<TData>(dataStr)
                    : ((JToken) entity.Data).ToObject<TData>());
        }

        private static OperationExecutionInfoEntity Convert<TData>(IOperationExecutionInfo<TData> model)
            where TData : class
        {
            return new OperationExecutionInfoEntity
            {
                Id = model.Id,
                OperationName = model.OperationName,
                Data = model.Data.ToJson()
            };
        }
    }
}