using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Contracts.Enums;
using Refit;

namespace MarginTrading.SettingsService.Contracts
{
    [PublicAPI]
    public interface IAssetPairsApi
    {
        [Get("/api/assetPairs")]
        Task<List<AssetPairContract>> List(
            [Query, CanBeNull] string legalEntity = null,
            [Query] MatchingEngineModeContract? matchingEngineMode = null);


        [Post("/api/assetPairs")]
        Task<AssetPairContract> Insert([Body] AssetPairContract assetPair);


        [ItemCanBeNull]
        [Get("/api/assetPairs/{assetPairId}")]
        Task<AssetPairContract> Get([NotNull] string assetPairId);


        [Put("/api/assetPairs/{assetPairId}")]
        Task<AssetPairContract> Update([NotNull] string assetPairId, [Body] AssetPairContract assetPair);


        [Delete("/api/assetPairs/{assetPairId}")]
        Task Delete([NotNull] string assetPairId);
    }
}