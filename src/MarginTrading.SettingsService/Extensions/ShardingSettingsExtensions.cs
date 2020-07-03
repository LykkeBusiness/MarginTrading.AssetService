// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.SettingsService.Contracts.Candles;
using MarginTrading.SettingsService.Settings.Candles;

namespace MarginTrading.SettingsService.Extensions
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