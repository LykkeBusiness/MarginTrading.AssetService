// Copyright (c) 2019 Lykke Corp.

using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.AzureRepositories.Entities
{
    public class MarketEntity : SimpleAzureEntity, IMarket
    {
        public static readonly string Pk = "Markets"; 
        internal override string SimplePartitionKey => Pk;
        
        // Id comes from parent type
        public string Name { get; set; }
    }
}