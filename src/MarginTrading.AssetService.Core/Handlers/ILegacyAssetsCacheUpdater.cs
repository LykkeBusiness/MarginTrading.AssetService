using System;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Handlers
{
    public interface ILegacyAssetsCacheUpdater
    {
        Task HandleProductUpserted(Product product, DateTime timestamp);

        Task HandleProductRemoved(string productId, DateTime timestamp);

        Task HandleMarketSettingsUpdated(MarketSettings marketSettings, DateTime timestamp);

        Task HandleTickFormulaUpdated(TickFormula tickFormula, DateTime timestamp);

        Task HandleProductCategoryUpdated(ProductCategory productCategory, DateTime timestamp);

        Task HandleClientProfileSettingsUpdated(ClientProfileSettings clientProfileSettings, DateTime timestamp);

        Task HandleCurrencyUpdated(string oldInterestRateMdsCode, Currency currency, DateTime timestamp);

        Task HandleUnderlyingUpdated(string oldMdsCode, UnderlyingsCacheModel underlying, DateTime timestamp);

        Task HandleClientProfileChanged(DateTime timestamp);

        Task HandleAssetTypeUpdated(AssetType assetType, DateTime timestamp);

        Task UpdateAll(DateTime timestamp);
    }
}