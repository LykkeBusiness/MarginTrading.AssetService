using AutoMapper;

using MarginTrading.AssetService.Contracts.Scheduling;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;

using Newtonsoft.Json;

namespace MarginTrading.AssetService.Services.Mapping
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleSettings, ScheduleSettingsContract>();
            CreateMap<ScheduleSettings, CompiledScheduleSettingsContract>();
            CreateMap<ScheduleConstraint, ScheduleConstraintContract>().ReverseMap();
            CreateMap<ScheduleConstraint, string>().ConvertUsing(sc => JsonConvert.SerializeObject(sc));
            CreateMap<IScheduleSettings, ScheduleSettings>();
            CreateMap<IScheduleSettings, CompiledScheduleSettingsContract>();
        }
    }
}