using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public class ClientProfileSettingsCache : IClientProfileSettingsCache
    {
        private readonly IClientProfileSettingsApi _clientProfileSettingsApi;

        private Dictionary<string, ClientProfileSettingsContract> _cache =
            new Dictionary<string, ClientProfileSettingsContract>();

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public ClientProfileSettingsCache(IClientProfileSettingsApi clientProfileSettingsApi)
        {
            _clientProfileSettingsApi = clientProfileSettingsApi;
        }

        public void Start()
        {
            _lockSlim.EnterWriteLock();
            try
            {
                var response = _clientProfileSettingsApi
                    .GetClientProfileSettingsByRegulationAsync()
                    .GetAwaiter()
                    .GetResult();

                _cache = response.ClientProfileSettings.ToDictionary(GetKey, x => x);
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
                var isInCache = _cache.ContainsKey(GetKey(clientProfileSettings));
                if (isInCache)
                    _cache.Remove(clientProfileSettings.ClientProfileId);
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public ClientProfileSettingsContract GetByIds(string profileId, string assetType)
        {
            _cache.TryGetValue(GetKey(profileId, assetType), out var result);

            return result;
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