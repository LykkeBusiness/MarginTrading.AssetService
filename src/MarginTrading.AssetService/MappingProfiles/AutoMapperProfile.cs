using System;
using System.Collections.Generic;
using AutoMapper;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.AssetTypes;
using MarginTrading.AssetService.Contracts.ClientProfiles;
using MarginTrading.AssetService.Contracts.ClientProfileSettings;
using MarginTrading.AssetService.Contracts.Scheduling;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using Newtonsoft.Json;
using AuditContract = MarginTrading.AssetService.Contracts.Audit.AuditContract;

namespace MarginTrading.AssetService.MappingProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Client profile Settings
            CreateMap<ClientProfileSettings, ClientProfileSettingsContract>();
            CreateMap<UpdateClientProfileSettingsRequest, ClientProfileSettings>()
                .ForMember(x => x.RegulatoryProfileId, opt => opt.Ignore())
                .ForMember(x => x.RegulatoryTypeId, opt => opt.Ignore());

            //Client profiles
            CreateMap<ClientProfile, ClientProfileContract>();
            CreateMap<AddClientProfileRequest, ClientProfileWithTemplate>();
            CreateMap<UpdateClientProfileRequest, ClientProfile>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            //Asset types
            CreateMap<AssetType, AssetTypeContract>();
            CreateMap<AddAssetTypeRequest, AssetTypeWithTemplate>();
            CreateMap<UpdateAssetTypeRequest, AssetType>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            //Audit
            CreateMap<IAuditModel, AuditContract>();

            CreateMap<HashSet<string>, string>().ConvertUsing(x => JsonConvert.SerializeObject(x));
            CreateMap<string, HashSet<string>>().ConvertUsing(x => JsonConvert.DeserializeObject<HashSet<string>>(x));
            CreateMap<List<string>, string>().ConvertUsing(x => JsonConvert.SerializeObject(x));
            CreateMap<string, List<string>>().ConvertUsing(x => JsonConvert.DeserializeObject<List<string>>(x));
            CreateMap<ScheduleConstraint, ScheduleConstraintContract>().ReverseMap();
            CreateMap<ScheduleConstraint, string>().ConvertUsing(x => JsonConvert.SerializeObject(x));
            CreateMap<string, ScheduleConstraint>().ConvertUsing(x => JsonConvert.DeserializeObject<ScheduleConstraint>(x));
            CreateMap<bool?, string>().ConstructUsing(x => x != null ? x.ToString() : "");
            CreateMap<string, bool?>().ConstructUsing((x,y) => bool.TryParse(x, out var parsed) ? parsed : (bool?)null);
            CreateMap<TimeSpan?, string>().ConstructUsing(x => JsonConvert.SerializeObject(x));
            CreateMap<string, TimeSpan?>().ConstructUsing((x,y) => TimeSpan.TryParse(x, out var parsed) ? parsed : (TimeSpan?)null);
            CreateMap<FreezeInfo, string>().ConvertUsing(x => JsonConvert.SerializeObject(x));
            CreateMap<string, FreezeInfo>().ConvertUsing(s =>
            string.IsNullOrEmpty(s) ? new FreezeInfo() : JsonConvert.DeserializeObject<FreezeInfo>(s));
            CreateMap<string, FreezeInfoContract>().ConvertUsing(s =>
            string.IsNullOrEmpty(s) ? new FreezeInfoContract() : JsonConvert.DeserializeObject<FreezeInfoContract>(s));
        }
    }
}