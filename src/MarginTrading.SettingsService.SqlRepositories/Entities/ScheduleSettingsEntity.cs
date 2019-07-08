// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using Newtonsoft.Json;

namespace MarginTrading.SettingsService.SqlRepositories.Entities
{
    public class ScheduleSettingsEntity : IScheduleSettings
    {
        public string Id { get; set; }
        public int Rank { get; set; }
        public string AssetPairRegex { get; set; }
        HashSet<string> IScheduleSettings.AssetPairs => JsonConvert.DeserializeObject<HashSet<string>>(AssetPairs); 
        public string AssetPairs { get; set; }
        public string MarketId { get; set; }
        
        public bool? IsTradeEnabled { get; set; }
        TimeSpan? IScheduleSettings.PendingOrdersCutOff => TimeSpan.TryParse(PendingOrdersCutOff, out var parsed) 
            ? parsed : (TimeSpan?)null; 
        public string PendingOrdersCutOff { get; set; }
        
        ScheduleConstraint IScheduleSettings.Start => JsonConvert.DeserializeObject<ScheduleConstraint>(Start); 
        public string Start { get; set; }
        ScheduleConstraint IScheduleSettings.End => JsonConvert.DeserializeObject<ScheduleConstraint>(End); 
        public string End { get; set; }
    }
}