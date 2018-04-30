using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.SettingsReader;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces;

namespace MarginTrading.SettingsService.AzureRepositories
{
    public class GenericAzureCrudRepository<TD, TE> : IGenericCrudRepository<TD>
        where TE: SimpleAzureEntity, new()
    {
        protected readonly INoSQLTableStorage<TE> TableStorage;
        protected readonly IConvertService ConvertService;
        protected readonly ILog Log;

        protected readonly Action<IMappingOperationOptions<TD, TE>> DefaultAzureMappingOpts = opts => opts
            .ConfigureMap(MemberList.Source).ForMember(e => e.ETag, e => e.UseValue("*"));
        
        protected GenericAzureCrudRepository(
            ILog log,
            IConvertService convertService,
            IReloadingManager<string> connectionStringManager,
            string tableName)
        {
            TableStorage = AzureTableStorage<TE>.Create(
                connectionStringManager,
                tableName,
                Log,
                new TimeSpan(1, 0, 0)
            );

            Log = log;
            ConvertService = convertService;
        }
        
        public virtual async Task<IReadOnlyList<TD>> GetAsync()
        {
            var data = await TableStorage.GetDataAsync();
            return data.Select(x => ConvertService.Convert<TE, TD>(x)).ToList();
        }

        public virtual async Task<IReadOnlyList<TD>> GetAsync(string partitionKey)
        {
            var data = await TableStorage.GetDataAsync(partitionKey);
            return data.Select(x => ConvertService.Convert<TE, TD>(x)).ToList();
        }
        
        public virtual async Task<IReadOnlyList<TD>> GetAsync(Func<TD, bool> filter)
        {
            var data = await TableStorage.GetDataAsync(x => filter(ConvertService.Convert<TE, TD>(x)));

            return data.Select(x => ConvertService.Convert<TE, TD>(x)).ToList();
        }

        public virtual async Task<TD> GetAsync(string rowKey, string partitionKey = null)
        {
            var entity = await TableStorage.GetDataAsync(GetPartitionKey(partitionKey), rowKey);

            return entity == null ? default(TD) : ConvertService.Convert<TE, TD>(entity);
        }

        public virtual async Task InsertAsync(TD obj)
        {
            var entity = ConvertService.Convert<TD, TE>(obj, DefaultAzureMappingOpts);
            entity.SetKeys();
                
            await TableStorage.InsertAsync(entity);
        }

        public virtual async Task ReplaceAsync(TD obj)
        {
            var entity = ConvertService.Convert<TD, TE>(obj, DefaultAzureMappingOpts);
            entity.SetKeys();
            
            await TableStorage.ReplaceAsync(entity);
        }

        public virtual async Task<bool> DeleteAsync(string rowKey, string partitionKey = null)
        {
            return await TableStorage.DeleteIfExistAsync(GetPartitionKey(partitionKey), rowKey);
        }

        private static string GetPartitionKey(string partitionKey)
        {
            if (!string.IsNullOrEmpty(partitionKey)) 
                return partitionKey;
            
            var instance = Activator.CreateInstance<TE>();
            if (string.IsNullOrEmpty(instance.SimplePartitionKey))
            {
                throw new Exception("Partition key must be passed explicitly or set via SimplePartitionKey.");
            }

            return instance.SimplePartitionKey;
        }
    }
}