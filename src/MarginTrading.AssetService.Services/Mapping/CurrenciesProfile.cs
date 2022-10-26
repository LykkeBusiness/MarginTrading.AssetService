using AutoMapper;

using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Services.Mapping
{
    public class CurrenciesProfile : Profile
    {
        public CurrenciesProfile()
        {
            CreateMap<AddCurrencyRequest, Currency>()
                .ForMember(dest => dest.Accuracy, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore());
            CreateMap<UpdateCurrencyRequest, Currency>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Accuracy, opt => opt.Ignore())
                .ForMember(dest => dest.Timestamp, opt => opt.Ignore());
            CreateMap<CurrenciesErrorCodes, CurrenciesErrorCodesContract>();
            CreateMap<Currency, CurrencyContract>().ReverseMap();
        }
    }
}