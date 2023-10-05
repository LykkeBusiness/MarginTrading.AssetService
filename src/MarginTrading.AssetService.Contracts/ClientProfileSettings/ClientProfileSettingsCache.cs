using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Contracts.ClientProfileSettings
{
    public class ClientProfileSettingsCache : BaseCache<ClientProfileSettingsContract>, IClientProfileSettingsCache
    {
        private readonly IClientProfileSettingsApi _clientProfileSettingsApi;
        private readonly ILogger<ClientProfileSettingsCache> _logger;

        public ClientProfileSettingsCache(IClientProfileSettingsApi clientProfileSettingsApi, 
            ILogger<ClientProfileSettingsCache> logger)
        {
            _clientProfileSettingsApi = clientProfileSettingsApi ??
                                        throw new ArgumentNullException(nameof(clientProfileSettingsApi));
            _logger = logger;
        }

        public override async Task StartAsync()
        {
            _logger?.LogInformation("Starting client profile settings cache ...");

            await _initializeLock.WaitAsync();
            try
            {
                var response = await _clientProfileSettingsApi
                    .GetClientProfileSettingsByRegulationAsync();

                _cache = new ConcurrentDictionary<string, ClientProfileSettingsContract>(
                    response.ClientProfileSettings.ToDictionary(GetKey, x => x));
            }
            finally
            {
                _initializeLock.Release();
                _logger?.LogInformation("Client profile settings cache has been started. Items: {Count}", _cache.Count);
            }
        }

        public IReadOnlyList<ClientProfileSettingsContract> GetByAssetType(string assetType, bool availableOnly = false)
        {
            if (string.IsNullOrEmpty(assetType))
                throw new ArgumentNullException(nameof(assetType));

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

            bool Filter(ClientProfileSettingsContract src)
            {
                return src.AssetTypeId == assetType && (!availableOnly || src.IsAvailable);
            }
        }

        protected override string GetKey(ClientProfileSettingsContract clientProfileSettings)
        {
            return $"{clientProfileSettings.ClientProfileId}_{clientProfileSettings.AssetTypeId}";
        }
    }
}