// Copyright (c) 2019 Lykke Corp.

using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using Refit;

namespace MarginTrading.SettingsService.Contracts
{
    /// <summary>
    /// Trading conditions management
    /// </summary>
    [PublicAPI]
    public interface ITradingConditionsApi
    {
        /// <summary>
        /// Get the list of trading conditions
        /// </summary>
        [Get("/api/tradingConditions")]
        Task<List<TradingConditionContract>> List([Query] bool? isDefault = null);

        /// <summary>
        /// Create new trading condition
        /// </summary>
        [Post("/api/tradingConditions")]
        Task<TradingConditionContract> Insert([Body] TradingConditionContract tradingCondition);

        /// <summary>
        /// Get the trading condition
        /// </summary>
        [ItemCanBeNull]
        [Get("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Get([NotNull] string tradingConditionId);
        
        /// <summary>
        /// Update the trading condition
        /// </summary>
        [Put("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Update(
            [NotNull] string tradingConditionId,
            [Body] TradingConditionContract tradingCondition);
    }
}
