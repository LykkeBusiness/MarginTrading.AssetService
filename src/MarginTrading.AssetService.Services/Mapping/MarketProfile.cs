using AutoMapper;

using Lykke.Snow.Common.WorkingDays;

using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.Market;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Services.Mapping
{
    public class MarketProfile : Profile
    {
        public MarketProfile()
        {
            CreateMap<MarketSchedule, MarketScheduleContract>();
            CreateMap<MarketSettings, MarketSettingsContract>()
                .ForMember(dest => dest.Open, opt => opt.MapFrom(x => x.MarketSchedule.Open))
                .ForMember(dest => dest.Close, opt => opt.MapFrom(x => x.MarketSchedule.Close))
                .ForMember(dest => dest.Timezone, opt => opt.MapFrom(x => x.MarketSchedule.TimeZoneId));
            CreateMap<MarketSettingsContract, MarketSettings>()
                .ForMember(dest => dest.MarketSchedule, opt => opt.MapFrom<MarketScheduleResolver>());

            CreateMap<AddMarketSettingsRequest, MarketSettingsCreateOrUpdateDto>();
            CreateMap<UpdateMarketSettingsRequest, MarketSettingsCreateOrUpdateDto>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            CreateMap<Market, MarketContract>();

            CreateMap<MarketSettingsErrorCodes, MarketSettingsErrorCodesContract>();

            CreateMap<IMarket, MarketContract>();
        }
    }
}