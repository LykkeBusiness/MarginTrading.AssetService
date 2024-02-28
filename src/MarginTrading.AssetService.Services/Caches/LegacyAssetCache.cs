using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services.Caches
{
    public class LegacyAssetCache : ILegacyAssetsCache
    {
        private readonly ILegacyAssetsService _legacyAssetsService;
        private readonly ILogger<LegacyAssetCache> _logger;

        private Dictionary<string, Asset> _cache = new Dictionary<string, Asset>();
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();
        public DateTime CacheInitTimestamp { get; private set; }

        public LegacyAssetCache(ILegacyAssetsService legacyAssetsService, ILogger<LegacyAssetCache> logger)
        {
            _legacyAssetsService = legacyAssetsService;
            _logger = logger;
        }

        public void Start()
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _logger.LogInformation("Asset(Legacy) Cache init started");
                CacheInitTimestamp = DateTime.UtcNow;

                var response = _legacyAssetsService.GetLegacyAssets().GetAwaiter().GetResult();

                _cache = response.ToDictionary(a => (string)a.AssetId, v => v);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public Asset GetById(string id)
        {
            _lockSlim.EnterReadLock();
            try
            {
                _cache.TryGetValue(id, out var result);

                return result;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public List<Asset> GetByFilter(Func<Asset, bool> filter)
        {
            _lockSlim.EnterReadLock();
            try
            {
                var result = _cache.Values.Where(filter).ToList();

                return result;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public List<Asset> GetAll()
        {
            _lockSlim.EnterReadLock();
            try
            {
                var result = _cache.Values.ToList();

                return result;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public void AddOrUpdateMultiple(IEnumerable<Asset> assets)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                foreach (var asset in assets)
                {
                    _cache[asset.AssetId] = asset;
                }
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void Remove(string assetId)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache.Remove(assetId);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }
    }
}