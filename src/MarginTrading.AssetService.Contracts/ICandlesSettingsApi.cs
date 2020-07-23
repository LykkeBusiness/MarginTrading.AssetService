// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Candles;
using Refit;

namespace MarginTrading.AssetService.Contracts
{
    /// <summary>
    /// The candles sharding shared settings
    /// </summary>
    [PublicAPI]
    public interface ICandlesSettingsApi
    {
        /// <summary>
        /// Get candles producer settings
        /// </summary>
        /// <returns></returns>
        [Get("/api/candles/producer")]
        Task<CandlesProducerSettingsContract> GetProducerSettingsAsync();

        /// <summary>
        /// Get candles consumer settings
        /// </summary>
        /// <param name="shardName">The shard name to get settings for</param>
        /// <returns></returns>
        [Get("/api/candles/consumer")]
        Task<CandlesConsumerSettingsContract> GetConsumerSettingsAsync([Query] string shardName);
    }
}