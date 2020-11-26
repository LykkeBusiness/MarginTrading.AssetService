// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.Audit;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.ErrorCodes;
using MarginTrading.AssetService.Contracts.ProductCategories;
using MarginTrading.AssetService.Contracts.Products;
using MarginTrading.AssetService.Contracts.Scheduling;
using MarginTrading.AssetService.Contracts.TickFormula;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using Newtonsoft.Json;
using AuditContract = MarginTrading.AssetService.Contracts.Audit.AuditContract;

namespace MarginTrading.AssetService.Services
{
    [UsedImplicitly]
    public class ConvertService : IConvertService
    {
        private readonly IMapper _mapper;

        public ConvertService()
        {
            _mapper = CreateMapper();
        }

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HashSet<string>, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, HashSet<string>>().ConvertUsing(JsonConvert.DeserializeObject<HashSet<string>>);
                cfg.CreateMap<List<string>, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, List<string>>().ConvertUsing(JsonConvert.DeserializeObject<List<string>>);
                cfg.CreateMap<ScheduleConstraint, ScheduleConstraintContract>().ReverseMap();
                cfg.CreateMap<ScheduleConstraint, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, ScheduleConstraint>().ConvertUsing(JsonConvert.DeserializeObject<ScheduleConstraint>);
                cfg.CreateMap<bool?, string>().ConstructUsing(x => x?.ToString() ?? "");
                cfg.CreateMap<string, bool?>().ConstructUsing(x => bool.TryParse(x, out var parsed) ? parsed : (bool?)null);
                cfg.CreateMap<TimeSpan?, string>().ConstructUsing(x => JsonConvert.SerializeObject(x));
                cfg.CreateMap<string, TimeSpan?>().ConstructUsing(x => TimeSpan.TryParse(x, out var parsed) ? parsed : (TimeSpan?)null);
                cfg.CreateMap<FreezeInfo, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, FreezeInfo>().ConvertUsing(s =>
                    string.IsNullOrEmpty(s) ? new FreezeInfo() : JsonConvert.DeserializeObject<FreezeInfo>(s));
                cfg.CreateMap<string, FreezeInfoContract>().ConvertUsing(s =>
                    string.IsNullOrEmpty(s) ? new FreezeInfoContract() : JsonConvert.DeserializeObject<FreezeInfoContract>(s));

                //Client profile Settings
                cfg.CreateMap<ClientProfileSettings, ClientProfileSettingsContract>().ReverseMap();
                cfg.CreateMap<UpdateClientProfileSettingsRequest, ClientProfileSettings>()
                    .ForMember(x => x.RegulatoryProfileId, opt => opt.Ignore())
                    .ForMember(x => x.RegulatoryTypeId, opt => opt.Ignore());
                cfg.CreateMap<CheckRegulationConstraintViolationRequest, RegulatorySettingsDto>();

                //Client profiles
                cfg.CreateMap<ClientProfile, ClientProfileContract>().ReverseMap();
                
                //Asset types
                cfg.CreateMap<AssetType, AssetTypeContract>();
                cfg.CreateMap<AddAssetTypeRequest, AssetTypeWithTemplate>();
                cfg.CreateMap<UpdateAssetTypeRequest, AssetType>()
                    .ForMember(x => x.Id, opt => opt.Ignore());
                
                //Audit
                cfg.CreateMap<IAuditModel, AuditContract>();
                cfg.CreateMap<GetAuditLogsRequest, AuditLogsFilterDto>();
                
                //Products
                cfg.CreateMap<Product, ProductContract>().ReverseMap();
                cfg.CreateMap<AddProductRequest, Product>();
                cfg.CreateMap<UpdateProductRequest, Product>();
                cfg.CreateMap<ProductFreezeInfo, ProductFreezeInfoContract>().ReverseMap();
                
                //ProductCategories
                cfg.CreateMap<ProductAndCategoryPairContract, ProductAndCategoryPair>();

                //MarketSettings
                cfg.CreateMap<MarketSettings, MarketSettingsContract>().ReverseMap();
                cfg.CreateMap<AddMarketSettingsRequest, MarketSettingsCreateOrUpdateDto>();
                cfg.CreateMap<UpdateMarketSettingsRequest, MarketSettingsCreateOrUpdateDto>()
                    .ForMember(x => x.Id, opt => opt.Ignore());

                
                //Currencies
                cfg.CreateMap<AddCurrencyRequest, Currency>();
                cfg.CreateMap<UpdateCurrencyRequest, Currency>();
                cfg.CreateMap<CurrenciesErrorCodes, CurrenciesErrorCodesContract>();
                cfg.CreateMap<Currency, CurrencyContract>().ReverseMap();

                //Tick formula
                cfg.CreateMap<ITickFormula, TickFormulaContract>().ReverseMap();
                cfg.CreateMap<TickFormula, TickFormulaContract>().ReverseMap();
                cfg.CreateMap<AddTickFormulaRequest, ITickFormula>();
                cfg.CreateMap<UpdateTickFormulaRequest, ITickFormula>()
                    .ForMember(x => x.Id, opt => opt.Ignore());

                //Underlying
                cfg.CreateMap<UnderlyingContract, UnderlyingsCacheModel>();
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