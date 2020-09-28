using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Log;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Requests;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services.Caches
{
    public class UnderlyingsCache : IUnderlyingsCache
    {
        private readonly IUnderlyingsApi _underlyingsApi;
        private readonly IConvertService _convertService;
        private readonly ILog _log;

        private Dictionary<string, UnderlyingsCacheModel> _cache = new Dictionary<string, UnderlyingsCacheModel>();
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public UnderlyingsCache(IUnderlyingsApi underlyingsApi,IConvertService convertService, ILog log)
        {
            _underlyingsApi = underlyingsApi;
            _convertService = convertService;
            _log = log;
        }

        public void Start()
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _log.WriteInfo(nameof(UnderlyingsCache), nameof(Start), "Underlyings Cache init started.");

                var response = _underlyingsApi.GetAllAsync(new GetUnderlyingsRequest{MdsCodes = null},0, 0).GetAwaiter().GetResult();

                _log.WriteInfo(nameof(UnderlyingsCache), nameof(Start), $"{response.Underlyings.Count} underlyings read.");

                _cache = response.Underlyings.ToDictionary(u => u.MdsCode,
                    v => _convertService.Convert<UnderlyingContract, UnderlyingsCacheModel>(v));
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
                    _log.WriteWarning(nameof(UnderlyingsCache), nameof(GetByMdsCode), $"Cannot find underlying in chache by mdsCode: {mdsCode}");

                return result;
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
                var isInCache = _cache.TryGetValue(underlying.MdsCode, out var result);
                if (isInCache)
                    _cache[underlying.MdsCode] = underlying;
                else
                    _cache.Add(underlying.MdsCode, underlying);
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
                var isInCache = _cache.TryGetValue(underlying.MdsCode, out var result);
                if (isInCache)
                    _cache.Remove(underlying.MdsCode);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }
    }
}