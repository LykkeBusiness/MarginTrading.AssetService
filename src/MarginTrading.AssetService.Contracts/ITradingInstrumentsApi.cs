// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.TradingConditions;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// Trading instruments management
    /// </summary>
    [PublicAPI]
    public interface ITradingInstrumentsApi
    {
        /// <summary>
        /// Get the list of trading instruments
        /// </summary>
        [Get("/api/tradingInstruments")]
        Task<List<TradingInstrumentContract>> List([Query, CanBeNull] string tradingConditionId);
        
        /// <summary>
        /// Get the list of trading instruments with optional pagination
        /// </summary>
        [Get("/api/tradingInstruments/by-pages")]
        Task<PaginatedResponseContract<TradingInstrumentContract>> ListByPages(
            [Query, CanBeNull] string tradingConditionId,
            [Query, CanBeNull] int? skip = null, [Query, CanBeNull] int? take = null);

        /// <summary>
        /// Get trading instrument
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/tradingInstruments/{tradingConditionId}/{assetPairId}")]
        Task<TradingInstrumentContract> Get(
            [NotNull] string tradingConditionId,
            [NotNull] string assetPairId);
    }
}
