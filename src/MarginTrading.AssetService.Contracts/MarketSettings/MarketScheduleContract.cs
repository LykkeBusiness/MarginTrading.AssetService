using System.Collections.Generic;
using Lykke.Snow.Common.WorkingDays;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.MarketSettings
{
    [MessagePackObject]
    public class MarketScheduleContract
    {
        [Key(0)]
        public List<WorkingDay> HalfWorkingDays { get; set; }
    }
}