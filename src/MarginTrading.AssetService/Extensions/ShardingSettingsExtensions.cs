// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.AssetService.Contracts.Candles;
using MarginTrading.AssetService.Settings.Candles;

namespace MarginTrading.AssetService.Extensions
{
    public static class ShardingSettingsExtensions
    {
        public static CandlesShardSettingsContract ToContract(this CandlesPublicationShard shard)
        {
            if (shard == null)
                throw new ArgumentNullException(nameof(shard));
            
            return new CandlesShardSettingsContract {Name = shard.Name, Pattern = shard.Pattern};
        }

        public static CandlesConsumerSettingsContract ToConsumerContract(this CandlesPublicationShard shard)
        {
            if (shard == null)
                throw new ArgumentNullException(nameof(shard));

            return new CandlesConsumerSettingsContract {Name = shard.Name, Pattern = shard.Pattern};
        }
    }
}