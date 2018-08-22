using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Common;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.AzureRepositories.Entities;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class OperationExecutionInfoRepository : IOperationExecutionInfoRepository
    {
        private readonly INoSQLTableStorage<OperationExecutionInfoEntity> _tableStorage;
        private readonly ILog _log;
        private readonly ISystemClock _systemClock;
        private readonly bool _enableOperationsLogs = true;

        public OperationExecutionInfoRepository(IReloadingManager<string> connectionStringManager, 
            ILog log, ISystemClock systemClock)
        {
            _tableStorage = AzureTableStorage<OperationExecutionInfoEntity>.Create(
                connectionStringManager,
                "SettingsServiceExecutionInfo",
                log);
            _log = log.CreateComponentScope(nameof(OperationExecutionInfoRepository));
            _systemClock = systemClock;
        }
        
        public async Task<IOperationExecutionInfo<TData>> GetOrAddAsync<TData>(
            string operationName, string operationId, Func<IOperationExecutionInfo<TData>> factory) where TData : class
        {
            var entity = await _tableStorage.GetOrInsertAsync(
                partitionKey: OperationExecutionInfoEntity.GeneratePartitionKey(operationName),
                rowKey: OperationExecutionInfoEntity.GeneratePartitionKey(operationId),
                createNew: () =>
                {
                    var result = Convert(factory());
                    result.LastModified = _systemClock.UtcNow.UtcDateTime;
                    return result;
                });
                
            return Convert<TData>(entity);
        }

        public async Task<IOperationExecutionInfo<TData>> GetAsync<TData>(string operationName, string id)
            where TData : class
        {
            var obj = await _tableStorage.GetDataAsync(
                          OperationExecutionInfoEntity.GeneratePartitionKey(operationName),
                          OperationExecutionInfoEntity.GenerateRowKey(id)) ?? throw new InvalidOperationException(
                          $"Operation execution info for {operationName} #{id} not yet exists");
            
            return Convert<TData>(obj);
        }

        public async Task Save<TData>(IOperationExecutionInfo<TData> executionInfo) where TData : class
        {
            var entity = Convert(executionInfo);
            entity.LastModified = _systemClock.UtcNow.UtcDateTime;
            await _tableStorage.ReplaceAsync(entity);
        }

        private static IOperationExecutionInfo<TData> Convert<TData>(OperationExecutionInfoEntity entity)
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
                Data = model.Data.ToJson(),
            };
        }
    }
}