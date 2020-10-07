using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class MarketsService : IMarketsService
    {
        private readonly IMarketSettingsRepository _marketSettingsRepository;

        public MarketsService(IMarketSettingsRepository marketSettingsRepository)
        {
            _marketSettingsRepository = marketSettingsRepository;
        }

        public async Task<IMarket> GetByIdAsync(string id)
        {
            var result = await _marketSettingsRepository.GetByIdAsync(id);

            return result != null ? new Market(result.Id, result.Name) : null;
        }

        public async Task<IReadOnlyList<IMarket>> GetAllAsync()
        {
            var marketSettings = await _marketSettingsRepository.GetAllMarketSettingsAsync();

            return marketSettings.Select(x => new Market(x.Id, x.Name)).ToList();
        }
    }
}