using AutoMapper;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.SqlRepositories.Entities;

namespace MarginTrading.AssetService.SqlRepositories.MappingProfile
{
    public class DataLayerMappingProfile : Profile
    {
        public DataLayerMappingProfile()
        {
            CreateMap<AssetPairEntity, AssetPair>();
            CreateMap<AssetEntity, Asset>();
            CreateMap<IAsset, AssetEntity>();
            CreateMap<MarketEntity, Market>();
            CreateMap<MarketEntity, IMarket>().ReverseMap();
            CreateMap<ScheduleSettingsEntity, ScheduleSettings>().ReverseMap();
            CreateMap<IScheduleSettings, ScheduleSettingsEntity>().ReverseMap();
            CreateMap<TradingConditionEntity, TradingCondition>().ReverseMap();
            CreateMap<ITradingCondition, TradingConditionEntity>().ReverseMap();
            CreateMap<TradingInstrumentEntity, TradingInstrument>().ReverseMap();
            CreateMap<ITradingInstrument, TradingInstrumentEntity>();
            CreateMap<TradingRouteEntity, TradingRoute>();
            CreateMap<ITradingRoute, TradingRouteEntity>();
        }
    }
}