// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    public class AssetUpsertedEvent
    {
        public EventMetadata EventMetadata { get; set; }

        public Asset Asset { get; set; }

        public AssetUpdatedProperties PropertiesPriorValueIfUpdated { get; set; }
    }
}
