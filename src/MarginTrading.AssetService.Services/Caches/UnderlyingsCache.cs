using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Requests;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services.Caches
{
    public class UnderlyingsCache : IUnderlyingsCache
    {
        private readonly IUnderlyingsApi _underlyingsApi;
        private readonly IConvertService _convertService;
        private readonly ILogger<UnderlyingsCache> _logger;

        private Dictionary<string, UnderlyingsCacheModel> _cache = new Dictionary<string, UnderlyingsCacheModel>();
        private HashSet<string> _isins = new HashSet<string>();
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public UnderlyingsCache(IUnderlyingsApi underlyingsApi, IConvertService convertService, ILogger<UnderlyingsCache> logger)
        {
            _underlyingsApi = underlyingsApi;
            _convertService = convertService;
            _logger = logger;
        }

        public void Start()
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _logger.LogInformation("Underlyings cache init started");

                var response = _underlyingsApi.GetAllAsync(new GetUnderlyingsRequestV2 { MdsCodes = null, Take = 0, Skip = 0 }).GetAwaiter().GetResult();

                _logger.LogInformation("{UnderlyingCount} underlyings read", response?.Underlyings?.Count ?? 0);

                if (response?.Underlyings != null)
                {
                    _cache = response.Underlyings.ToDictionary(
                        u => u.MdsCode,
                        v => _convertService.Convert<UnderlyingApiContract, UnderlyingsCacheModel>(v));
                }
                
                InitIsinsUnsafely();
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public UnderlyingsCacheModel GetByMdsCode(string mdsCode)
        {
            _lockSlim.EnterReadLock();
            try
            {
                var isInCache = _cache.TryGetValue(mdsCode, out var result);
                if (!isInCache)
                    _logger.LogWarning("Cannot find underlying in cache by mdsCode: {MdsCode}", mdsCode);

                return result;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public bool IsinExists(string isin)
        {
            _lockSlim.EnterReadLock();
            try
            {
                return _isins.Contains(isin);
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public void AddOrUpdateByMdsCode(UnderlyingsCacheModel underlying)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache[underlying.MdsCode] = underlying;
                InitIsinsUnsafely();
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void AddOrUpdateByChangedMdsCode(string oldMdsCode, UnderlyingsCacheModel underlying)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache.Remove(oldMdsCode);

                _cache.TryAdd(underlying.MdsCode, underlying);
                InitIsinsUnsafely();
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void Remove(UnderlyingsCacheModel underlying)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache.Remove(underlying.MdsCode);
                InitIsinsUnsafely();
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        private void InitIsinsUnsafely()
        {
            _isins = _cache.Select(x => x.Value.Isin).ToHashSet();
        }
    }
}