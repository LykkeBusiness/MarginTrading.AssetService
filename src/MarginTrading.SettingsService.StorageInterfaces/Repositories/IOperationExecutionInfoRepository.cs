using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IOperationExecutionInfoRepository
    {
        Task<IOperationExecutionInfo<TData>> GetOrAddAsync<TData>(string operationName,
            string operationId, Func<IOperationExecutionInfo<TData>> factory) where TData : class;

        [ItemCanBeNull]
        Task<IOperationExecutionInfo<TData>> GetAsync<TData>(string operationName, string id) where TData : class;
        
        Task Save<TData>(IOperationExecutionInfo<TData> executionInfo) where TData : class;
    }
}