// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Common;
using Lykke.SettingsReader;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using MoreLinq;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.AzureRepositories.Repositories
{
    public class BlobRepository : IMarginTradingBlobRepository
    {
        private readonly IBlobStorage _blobStorage;

        public BlobRepository(IReloadingManager<string> connectionStringManager)
        {
            _blobStorage = AzureBlobStorage.Create(connectionStringManager);
            
            throw new NotImplementedException("does not support azure");
        }
 
        public T Read<T>(string blobContainer, string key)
        {
            if (_blobStorage.HasBlobAsync(blobContainer, key).Result)
            {
                var data = _blobStorage.GetAsync(blobContainer, key).Result.ToBytes();
                var str = Encoding.UTF8.GetString(data);

                return JsonConvert.DeserializeObject<T>(str);
            }

            return default(T);
        }


        public async Task MergeListAsync<T>(string blobContainer, string key, List<T> objects, 
            Func<T, string> selector)
        {
            var existing = Read<IEnumerable<T>>(blobContainer, key)?.ToList() ?? new List<T>();

            await WriteAsync(blobContainer, key, objects.Concat(existing.ExceptBy(objects, selector)));
        }

        public async Task<T> ReadAsync<T>(string blobContainer, string key)
        {
            if (_blobStorage.HasBlobAsync(blobContainer, key).Result)
            {
                var data = (await _blobStorage.GetAsync(blobContainer, key)).ToBytes();
                var str = Encoding.UTF8.GetString(data);

                return JsonConvert.DeserializeObject<T>(str);
            }

            return default(T);
        }

        public async Task WriteAsync<T>(string blobContainer, string key, T obj)
        {
            var data = JsonConvert.SerializeObject(obj).ToUtf8Bytes();
            await _blobStorage.SaveBlobAsync(blobContainer, key, data);
        }
    }
}