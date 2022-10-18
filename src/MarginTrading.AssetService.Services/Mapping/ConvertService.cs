// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using AutoMapper;

using JetBrains.Annotations;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.Audit;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using Newtonsoft.Json;
using AuditContract = MarginTrading.AssetService.Contracts.Audit.AuditContract;
using AuditDataType = MarginTrading.AssetService.Core.Domain.AuditDataType;

namespace MarginTrading.AssetService.Services.Mapping
{
    [UsedImplicitly]
    public class ConvertService : IConvertService
    {
        private readonly IMapper _mapper;

        public ConvertService()
        {
            _mapper = CreateMapper();
        }
        
        public void AssertConfigurationIsValid()
        {
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HashSet<string>, string>().ConvertUsing(s => JsonConvert.SerializeObject(s));
                cfg.CreateMap<string, HashSet<string>>().ConvertUsing(h => JsonConvert.DeserializeObject<HashSet<string>>(h));
                cfg.CreateMap<List<string>, string>().ConvertUsing(l => JsonConvert.SerializeObject(l));
                cfg.CreateMap<string, List<string>>().ConvertUsing(s => JsonConvert.DeserializeObject<List<string>>(s));
                cfg.CreateMap<string, ScheduleConstraint>().ConvertUsing(s => JsonConvert.DeserializeObject<ScheduleConstraint>(s));
                cfg.CreateMap<bool?, string>().ConstructUsing((b, ctx) => b?.ToString() ?? "");
                cfg.CreateMap<string, bool?>().ConvertUsing<NullBooleanTypeConverter>();
                cfg.CreateMap<TimeSpan?, string>().ConstructUsing(x => JsonConvert.SerializeObject(x));
                cfg.CreateMap<string, TimeSpan?>().ConvertUsing<NullTimespanTypeConverter>();
                cfg.CreateMap<FreezeInfo, string>().ConvertUsing(fi => JsonConvert.SerializeObject(fi));
                cfg.CreateMap<string, FreezeInfo>().ConvertUsing(s => string.IsNullOrEmpty(s) ? new FreezeInfo() : JsonConvert.DeserializeObject<FreezeInfo>(s));
                cfg.CreateMap<string, FreezeInfoContract>().ConvertUsing(s => string.IsNullOrEmpty(s) ? new FreezeInfoContract() : JsonConvert.DeserializeObject<FreezeInfoContract>(s));

                //Asset types
                cfg.CreateMap<AssetType, AssetTypeContract>().ReverseMap();
                cfg.CreateMap<AddAssetTypeRequest, AssetTypeWithTemplate>();
                cfg.CreateMap<UpdateAssetTypeRequest, AssetType>()
                    .ForMember(x => x.Id, opt => opt.Ignore());
                
                //Audit
                cfg.CreateMap<IAuditModel<AuditDataType>, AuditContract>();
                cfg.CreateMap<GetAuditLogsRequest, AuditTrailFilter<AuditDataType>>();
                
                //ProductCategories
                cfg.CreateMap<ProductCategory, ProductCategoryContract>();
                cfg.CreateMap<ProductAndCategoryPairContract, ProductAndCategoryPair>();

                //Tick formula
                cfg.CreateMap<ITickFormula, TickFormulaContract>().ReverseMap();
                cfg.CreateMap<TickFormula, TickFormulaContract>().ReverseMap();
                cfg.CreateMap<AddTickFormulaRequest, ITickFormula>();
                cfg.CreateMap<AddTickFormulaRequest, TickFormula>();
                cfg.CreateMap<UpdateTickFormulaRequest, ITickFormula>()
                    .ForMember(x => x.Id, opt => opt.Ignore());
                cfg.CreateMap<UpdateTickFormulaRequest, TickFormula>()
                    .ForMember(x => x.Id, opt => opt.Ignore());

                //Underlying
                cfg.CreateMap<UnderlyingContract, UnderlyingsCacheModel>();
                
                cfg.AddProfile<TradingConditionsProfile>();
                cfg.AddProfile<ProductsProfile>();
                cfg.AddProfile<CurrenciesProfile>();
                cfg.AddProfile<ScheduleProfile>();
                cfg.AddProfile<MarketProfile>();
            }).CreateMapper();
        }

        public TResult Convert<TSource, TResult>(TSource source,
            Action<IMappingOperationOptions<TSource, TResult>> opts)
        {
            return _mapper.Map(source, opts);
        }

        public TResult Convert<TSource, TResult>(TSource source)
        {
            return _mapper.Map<TSource, TResult>(source);
        }
    }
}