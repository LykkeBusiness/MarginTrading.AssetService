using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public class ClientProfileSettingsCache : IClientProfileSettingsCache
    {
        private readonly IClientProfileSettingsApi _clientProfileSettingsApi;
        private readonly ILogger<ClientProfileSettingsCache> _logger;

        private Dictionary<string, ClientProfileSettingsContract> _cache =
            new Dictionary<string, ClientProfileSettingsContract>();

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public ClientProfileSettingsCache(IClientProfileSettingsApi clientProfileSettingsApi, 
            ILogger<ClientProfileSettingsCache> logger)
        {
            _clientProfileSettingsApi = clientProfileSettingsApi;
            _logger = logger;
        }

        public void Start()
        {
            _logger?.LogDebug("Starting client profile settings cache ...");
            
            _lockSlim.EnterWriteLock();
            try
            {
                var response = _clientProfileSettingsApi
                    .GetClientProfileSettingsByRegulationAsync()
                    .GetAwaiter()
                    .GetResult();

                _cache = response.ClientProfileSettings.ToDictionary(GetKey, x => x);
                
                _logger?.LogDebug("Client profile settings cache has been started. Items: {count}", _cache.Count);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void AddOrUpdate(ClientProfileSettingsContract clientProfileSettings)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache[GetKey(clientProfileSettings)] = clientProfileSettings;

                _logger?.LogDebug(
                    "New entry has been added into client profile settings cache. Items: {count}. Entry: {clientProfileSettings}.", 
                    _cache.Count,
                    clientProfileSettings.ToJson());
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void Remove(ClientProfileSettingsContract clientProfileSettings)
        {
            _lockSlim.EnterWriteLock();
            try
            {
                _cache.Remove(GetKey(clientProfileSettings));

                _logger?.LogDebug(
                    "One entry has been removed from client profile settings cache. Items: {count}. Entry: {clientProfileSettings}",
                    _cache.Count,
                    clientProfileSettings.ToJson());
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public ClientProfileSettingsContract GetByIds(string profileId, string assetType)
        {
            _cache.TryGetValue(GetKey(profileId, assetType), out var result);

            if (result == null)
                throw new ArgumentOutOfRangeException(nameof(profileId),
                    $"Client profile settings not found for profile [{profileId}] and asset type [{assetType}]");

            return result;
        }

        public bool TryGetValue(string profileId, string assetType, out ClientProfileSettingsContract result)
        {
            _lockSlim.EnterReadLock();
            try
            {
                return _cache.TryGetValue(GetKey(profileId, assetType), out result);
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public IReadOnlyList<ClientProfileSettingsContract> GetByAssetType(string assetType, bool availableOnly = false)
        {
            if (string.IsNullOrEmpty(assetType))
                throw new ArgumentNullException(nameof(assetType));

            _lockSlim.EnterReadLock();
            try
            {
                var result = _cache.Values.Where(Filter).ToList();
                if (!result.Any())
                {
                    _logger?.LogDebug(
                        "Couldn't find client profile settings cache entries for assetType={assetType} with availableOnly={availableOnly}. Items: {count}",
                        assetType,
                        availableOnly,
                        _cache.Count);
                }

                return result;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }

            bool Filter(ClientProfileSettingsContract src)
            {
                return src.AssetTypeId == assetType && (!availableOnly || src.IsAvailable);
            }
        }

        private string GetKey(ClientProfileSettingsContract clientProfileSettings)
        {
            return GetKey(clientProfileSettings.ClientProfileId, clientProfileSettings.AssetTypeId);
        }

        private string GetKey(string clientProfileId, string assetType)
        {
            return $"{clientProfileId}_{assetType}";
        }
    }
}