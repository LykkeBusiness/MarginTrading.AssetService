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
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories.Repositories
{
    public class GenericAzureCrudRepository<TD, TE> : IGenericCrudRepository<TD>
        where TD: class
        where TE: SimpleAzureEntity, new()
    {
        protected readonly INoSQLTableStorage<TE> TableStorage;
        protected readonly IConvertService ConvertService;
        protected readonly ILog Log;
        
        protected readonly string PartitionKey;

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

            PartitionKey = Activator.CreateInstance<TE>().PartitionKey;
        }
        
        public virtual async Task<IReadOnlyList<TD>> GetAsync()
        {
            var data = await TableStorage.GetDataAsync();
            return data.Select(x => ConvertService.Convert<TE, TD>(x)).ToList();
        }
        
        public async Task<IReadOnlyList<TD>> GetAsync(Func<TD, bool> filter)
        {
            var data = await TableStorage.GetDataAsync(PartitionKey, x => filter(ConvertService.Convert<TE, TD>(x)));

            return data.Select(x => ConvertService.Convert<TE, TD>(x)).ToList();
        }

        public virtual async Task<TD> GetAsync(string id)
        {
            var entity = await TableStorage.GetDataAsync(PartitionKey, id);
            return ConvertService.Convert<TE, TD>(entity);
        }

        public virtual async Task InsertAsync(TD obj)
        {
            var entity = ConvertService.Convert<TD, TE>(obj, opts => opts.ConfigureMap(MemberList.Source));
            entity.SetRowKey();
                
            await TableStorage.InsertAsync(entity);
        }

        public virtual async Task ReplaceAsync(TD obj)
        {
            var entity = ConvertService.Convert<TD, TE>(obj, opts => opts.ConfigureMap(MemberList.Source));
            entity.SetRowKey();
            
            await TableStorage.ReplaceAsync(entity);
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            return await TableStorage.DeleteIfExistAsync(PartitionKey, id);
        }
    }
}