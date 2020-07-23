// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.AzureRepositories.Entities
{
    public class ScheduleSettingsEntity : SimpleAzureEntity, IScheduleSettings
    {
        public static readonly string Pk = "ScheduleSettings"; 
        internal override string SimplePartitionKey => Pk;
        
        // Id comes from parent type
        public int Rank { get; set; }
        public string AssetPairRegex { get; set; }
        HashSet<string> IScheduleSettings.AssetPairs => JsonConvert.DeserializeObject<HashSet<string>>(AssetPairs); 
        public string AssetPairs { get; set; }
        public string MarketId { get; set; }

        bool? IScheduleSettings.IsTradeEnabled => bool.TryParse(IsTradeEnabled, out var parsed)
            ? parsed : (bool?) null;
        public string IsTradeEnabled { get; set; }
        TimeSpan? IScheduleSettings.PendingOrdersCutOff => TimeSpan.TryParse(PendingOrdersCutOff, out var parsed) 
            ? parsed : (TimeSpan?)null; 
        public string PendingOrdersCutOff { get; set; }
        ScheduleConstraint IScheduleSettings.Start => JsonConvert.DeserializeObject<ScheduleConstraint>(Start); 
        public string Start { get; set; }
        ScheduleConstraint IScheduleSettings.End => JsonConvert.DeserializeObject<ScheduleConstraint>(End); 
        public string End { get; set; }
    }
}