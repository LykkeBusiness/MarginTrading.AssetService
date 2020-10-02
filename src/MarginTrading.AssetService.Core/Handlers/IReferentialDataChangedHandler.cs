using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Handlers
{
    public interface IReferentialDataChangedHandler
    {
        Task HandleProductUpserted(Product product);

        Task HandleMarketSettingsUpdated(MarketSettings marketSettings);

        Task HandleTickFormulaUpdated(TickFormula tickFormula);

        Task HandleProductCategoryUpdated(ProductCategory productCategory);

        Task HandleClientProfileSettingsUpdated(ClientProfileSettings clientProfileSettings);

        Task HandleCurrencyUpdated(string oldInterestRateMdsCode, Currency currency);

        Task HandleUnderlyingUpdated(string oldMdsCode, UnderlyingsCacheModel underlying);

        Task HandleClientProfileUpserted(ClientProfile old, ClientProfile updated);
    }
}