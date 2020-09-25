using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class TradingConditionsService : ITradingConditionsService
    {
        private readonly IClientProfilesRepository _clientProfilesRepository;
        private readonly DefaultTradingConditionsSettings _defaultTradingConditionsSettings;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;

        public TradingConditionsService(
            IClientProfilesRepository clientProfilesRepository,
            DefaultTradingConditionsSettings defaultTradingConditionsSettings,
            DefaultLegalEntitySettings defaultLegalEntitySettings)
        {
            _clientProfilesRepository = clientProfilesRepository;
            _defaultTradingConditionsSettings = defaultTradingConditionsSettings;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
        }

        public async Task<IReadOnlyList<ITradingCondition>> GetAsync()
        {
            var profiles = await _clientProfilesRepository.GetAllAsync();

            return profiles.Select(MapTradingCondition).ToList();
        }

        public async Task<ITradingCondition> GetAsync(string tradingConditionId)
        {
            var clientProfile = await _clientProfilesRepository.GetByIdAsync(tradingConditionId);

            if (clientProfile == null)
                return null;

            return MapTradingCondition(clientProfile);
        }

        public async Task<IReadOnlyList<ITradingCondition>> GetDefaultAsync()
        {
            var clientProfile = await _clientProfilesRepository.GetDefaultAsync();

            if (clientProfile == null)
                return null;

            return new List<ITradingCondition>
            {
                MapTradingCondition(clientProfile)
            };
        }
        private ITradingCondition MapTradingCondition(ClientProfile clientProfile)
        {
            return TradingCondition.CreateFromClientProfile(clientProfile,
                _defaultLegalEntitySettings.DefaultLegalEntity, _defaultTradingConditionsSettings.MarginCall1,
                _defaultTradingConditionsSettings.MarginCall2, _defaultTradingConditionsSettings.StopOut);
        }
    }
}