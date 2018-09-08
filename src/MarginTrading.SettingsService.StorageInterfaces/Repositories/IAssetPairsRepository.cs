using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IAssetPairsRepository
    {
        Task<IReadOnlyList<IAssetPair>> GetAsync(params string[] assetPairIds);
        Task<IAssetPair> GetAsync(string assetPairId);
        Task<IAssetPair> GetByBaseAssetPairAsync(string baseAssetPairId);
        Task<IAssetPair> GetByBaseAssetPairAndNotByIdAsync(string id, string baseAssetPairId);
        Task<IReadOnlyList<IAssetPair>> GetByLeAndMeModeAsync(string legalEntity = null, 
            string matchingEngineMode = null);
        Task<PaginatedResponse<IAssetPair>> GetByLeAndMeModeByPagesAsync(string legalEntity = null, 
            string matchingEngineMode = null, int? skip = null, int? take = null);
        Task<IAssetPair> GetByBaseQuoteAndLegalEntityAsync(string baseAssetId, string quoteAssetId, string legalEntity);
        
        /// <summary>
        /// Return null in case when insertion failed
        /// </summary>
        [ItemCanBeNull]
        Task<IAssetPair> InsertAsync(IAssetPair assetPair);
        /// <summary>
        /// Return null in case when insertion failed
        /// </summary>
        [ItemCanBeNull]
        Task<IReadOnlyList<IAssetPair>> InsertBatchAsync(IReadOnlyList<IAssetPair> assetPairs);
        Task DeleteAsync(string assetPairId);
        
        [ItemCanBeNull]
        Task<IAssetPair> UpdateAsync(IAssetPair assetPair);
        [ItemCanBeNull]
        Task<IReadOnlyList<IAssetPair>> UpdateBatchAsync(IReadOnlyList<IAssetPair> assetPairs);
        
        Task<IAssetPair> ChangeSuspendFlag(string assetPairId, bool suspendFlag);
    }
}
