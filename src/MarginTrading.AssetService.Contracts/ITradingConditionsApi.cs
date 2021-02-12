// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.TradingConditions;
using Refit;

namespace MarginTrading.AssetService.Contracts
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
        /// Get the trading condition
        /// </summary>
        [Obsolete("Use GetByClientProfileId instead.")]
        [Get("/api/tradingConditions/{tradingConditionId}")]
        Task<TradingConditionContract> Get([NotNull] string tradingConditionId);

        /// <summary>
        /// Get the trading condition by client profile id
        /// </summary>
        /// <param name="clientProfileId">The client profile id</param>
        /// <returns></returns>
        [Get("/api/tradingConditions/clientProfile/{clientProfileId}")]
        Task<TradingConditionContract> GetByClientProfileId([NotNull] string clientProfileId);
    }
}
