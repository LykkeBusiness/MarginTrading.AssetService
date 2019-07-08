// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class AssetEntity : SimpleAzureEntity, IAsset
    {
        public static readonly string Pk = "Assets"; 
        internal override string SimplePartitionKey => Pk;

        // Id comes from parent type
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }
}