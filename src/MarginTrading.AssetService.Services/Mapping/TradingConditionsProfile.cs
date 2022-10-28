using AutoMapper;

using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.TradingConditions;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Services.Mapping
{
    public class TradingConditionsProfile : Profile
    {
        public TradingConditionsProfile()
        {
            CreateMap<TradingCondition, TradingConditionContract>();
            CreateMap<ClientProfile, ClientProfileContract>().ReverseMap();
            CreateMap<ClientProfileSettings, ClientProfileSettingsContract>().ReverseMap();
            CreateMap<UpdateClientProfileSettingsRequest, ClientProfileSettings>()
                .ForMember(x => x.RegulatoryProfileId, opt => opt.Ignore())
                .ForMember(x => x.RegulatoryTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.ClientProfileId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetTypeId, opt => opt.Ignore());
            CreateMap<CheckRegulationConstraintViolationRequest, RegulatorySettingsDto>();
            CreateMap<ITradingCondition, TradingConditionContract>();
            CreateMap<ClientProfileSettingsContract, ClientProfileSettings>();
        }
    }
}