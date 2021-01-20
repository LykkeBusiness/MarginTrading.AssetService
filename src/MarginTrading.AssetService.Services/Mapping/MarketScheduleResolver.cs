using AutoMapper;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Mapping
{
    public class MarketScheduleResolver : IValueResolver<MarketSettingsContract, MarketSettings, MarketSchedule>
    {
        public MarketSchedule Resolve(MarketSettingsContract source,
            MarketSettings destination,
            MarketSchedule destMember,
            ResolutionContext context)
        {
            return new MarketSchedule(source.Open, source.Close, source.Timezone, source.MarketSchedule.HalfWorkingDays);
        }
    }
}