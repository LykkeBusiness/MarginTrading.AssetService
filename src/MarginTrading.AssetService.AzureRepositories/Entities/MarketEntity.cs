// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.AzureRepositories.Entities
{
    public class MarketEntity : SimpleAzureEntity, IMarket
    {
        public static readonly string Pk = "Markets"; 
        internal override string SimplePartitionKey => Pk;
        
        // Id comes from parent type
        public string Name { get; set; }
    }
}