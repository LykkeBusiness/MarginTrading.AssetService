// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace MarginTrading.AssetService.Contracts.Candles
{
    /// <summary>
    /// Candles producer sharding settings
    /// </summary>
    [PublicAPI]
    public class CandlesProducerSettingsContract
    {
        /// <summary>
        ///  The list of shards
        /// </summary>
        public IEnumerable<CandlesShardSettingsContract> Shards { get; set; }
        
        /// <summary>
        /// The default shard name
        /// </summary>
        public string DefaultShardName { get; set; }
    }
}