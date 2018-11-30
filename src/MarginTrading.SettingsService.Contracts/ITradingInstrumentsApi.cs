using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.Common;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using Refit;

namespace MarginTrading.SettingsService.Contracts
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
        [Get("/api/assetPairs/by-pages")]
        Task<PaginatedResponseContract<TradingInstrumentContract>> ListByPages(
            [Query, CanBeNull] string tradingConditionId,
            [Query, CanBeNull] int? skip = null, [Query, CanBeNull] int? take = null);

        /// <summary>
        /// Create new trading instrument
        /// </summary>
        [Post("/api/tradingInstruments")]
        Task<TradingInstrumentContract> Insert([Body] TradingInstrumentContract instrument);

        /// <summary>
        /// Assign trading instrument to a trading condition with default values
        /// </summary>
        [Post("/api/tradingInstruments/{tradingConditionId}")]
        Task<List<TradingInstrumentContract>> AssignCollection(
            [NotNull] string tradingConditionId,
            [Body] string[] instruments);
        
        /// <summary>
        /// Get trading instrument
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/tradingInstruments/{tradingConditionId}/{assetPairId}")]
        Task<TradingInstrumentContract> Get(
            [NotNull] string tradingConditionId,
            [NotNull] string assetPairId);

        /// <summary>
        /// Update the trading instrument
        /// </summary>
        [Put("/api/tradingInstruments/{tradingConditionId}/{assetPairId}")]
        Task<TradingInstrumentContract> Update(
            [NotNull] string tradingConditionId,
            [NotNull] string assetPairId,
            [Body] TradingInstrumentContract instrument);

        [Put("/api/tradingInstruments/{tradingConditionId}/batch")]
        Task<List<TradingInstrumentContract>> UpdateList(string tradingConditionId,
            TradingInstrumentContract[] instruments);

        /// <summary>
        /// Delete the trading instrument
        /// </summary>
        [Delete("/api/tradingInstruments/{tradingConditionId}/{assetPairId}")]
        Task Delete(
            [NotNull] string tradingConditionId,
            [NotNull] string assetPairId);
    }
}
