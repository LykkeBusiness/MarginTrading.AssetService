// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    /// <summary>
    /// Asset types cache
    /// </summary>
    public class AssetTypeCache : BaseCache<AssetTypeContract>, IAssetTypeCache
    {
        private readonly IAssetTypesApi _assetTypesApi;
        private readonly ILogger<AssetTypeCache> _logger;

        public AssetTypeCache(IAssetTypesApi assetTypesApi, ILogger<AssetTypeCache> logger)
        {
            _assetTypesApi = assetTypesApi ?? throw new System.ArgumentNullException(nameof(assetTypesApi));
            _logger = logger;
        }

        public override async Task StartAsync()
        {
            _logger?.LogInformation("Starting asset types cache ...");
            
            await _initializeLock.WaitAsync();
            try
            {
                var response = await _assetTypesApi.GetAssetTypesAsync();
                _cache = new ConcurrentDictionary<string, AssetTypeContract>(
                    response.AssetTypes.ToDictionary(x => x.Id, x => x));
            }
            finally
            {
                _initializeLock.Release();
                _logger?.LogInformation("Asset types cache has been started. Items: {Count}", _cache.Count);
            }
        }
        
        protected override string GetKey(AssetTypeContract item) => item.Id;
    }
}