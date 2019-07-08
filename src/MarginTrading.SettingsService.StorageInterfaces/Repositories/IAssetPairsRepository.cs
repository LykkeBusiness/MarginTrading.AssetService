// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.StorageInterfaces.Repositories
{
    public interface IAssetPairsRepository
    {
        Task<IReadOnlyList<IAssetPair>> GetAsync(params string[] assetPairIds);
        
        [ItemCanBeNull]
        Task<IAssetPair> GetAsync(string assetPairId);
        
        [ItemCanBeNull]
        Task<IAssetPair> GetByBaseAssetPairAsync(string baseAssetPairId);
        
        [ItemCanBeNull]
        Task<IAssetPair> GetByBaseAssetPairAndNotByIdAsync(string id, string baseAssetPairId);
        
        [ItemCanBeNull]
        Task<IAssetPair> GetByBaseQuoteAndLegalEntityAsync(string baseAssetId, string quoteAssetId, string legalEntity);
        
        Task<IReadOnlyList<IAssetPair>> GetByLeAndMeModeAsync(string legalEntity = null,
            string matchingEngineMode = null, string filter = null);
        
        Task<PaginatedResponse<IAssetPair>> GetByLeAndMeModeByPagesAsync(string legalEntity = null,
            string matchingEngineMode = null, string filter = null, int? skip = null, int? take = null);
        
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
        Task<IAssetPair> UpdateAsync(AssetPairUpdateRequest assetPairUpdateRequest);
        
        [ItemCanBeNull]
        Task<IReadOnlyList<IAssetPair>> UpdateBatchAsync(IReadOnlyList<AssetPairUpdateRequest> assetPairsUpdateRequest);
        
        Task<IAssetPair> ChangeSuspendFlag(string assetPairId, bool suspendFlag);
    }
}
