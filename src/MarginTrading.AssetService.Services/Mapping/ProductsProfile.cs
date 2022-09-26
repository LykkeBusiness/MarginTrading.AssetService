using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Contracts.TradingConditions;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Services.Mapping
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductContract>().ReverseMap();
            // todo: mapping should work without MemberList.None option
            CreateMap<AddProductRequest, Product>(MemberList.None)
                //For new products, the default value for the IsSuspended flag should be true.
                //see https://lykke-snow.atlassian.net/browse/LT-2875
                .ForMember(p => p.IsSuspended, o => o.MapFrom(src => true))
                .ForMember(p => p.Name, o => o.MapFrom(x => x.Name.Trim()));
            // todo: mapping should work without MemberList.None option
            CreateMap<UpdateProductRequest, Product>(MemberList.None)
                .ForMember(p => p.Name, o => o.MapFrom(x => x.Name.Trim()));
            CreateMap<ProductFreezeInfo, ProductFreezeInfoContract>().ReverseMap();
            CreateMap<ITradingInstrument, TradingInstrumentContract>()
                .ForMember(dest => dest.InitLeverage, opt => opt.MapFrom(x => (decimal) x.InitLeverage))
                .ForMember(dest => dest.MaintenanceLeverage, opt => opt.MapFrom(x => (decimal) x.MaintenanceLeverage))
                .ForMember(dest => dest.MarginRatePercent, opt => opt.MapFrom(x => x.MarginRate.Value));
            CreateMap<MatchingEngineMode, MatchingEngineModeContract>().ConvertUsingEnumMapping();
            CreateMap<FreezeInfo, FreezeInfoContract>();
            CreateMap<FreezeReason, FreezeReasonContract>().ConvertUsingEnumMapping();
            CreateMap<IAssetPair, AssetPairContract>();
        }
    }
}