// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using AutoMapper;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.Scheduling;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using Newtonsoft.Json;

namespace MarginTrading.SettingsService.Services
{
    [UsedImplicitly]
    public class ConvertService : IConvertService
    {
        private readonly IMapper _mapper = CreateMapper();

        private static IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                // todo: add some global configurations here?
                cfg.CreateMap<HashSet<string>, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, HashSet<string>>().ConvertUsing(JsonConvert.DeserializeObject<HashSet<string>>);
                cfg.CreateMap<List<string>, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, List<string>>().ConvertUsing(JsonConvert.DeserializeObject<List<string>>);
                cfg.CreateMap<ScheduleConstraint, ScheduleConstraintContract>().ReverseMap();
                cfg.CreateMap<ScheduleConstraint, string>().ConvertUsing(JsonConvert.SerializeObject);
                cfg.CreateMap<string, ScheduleConstraint>().ConvertUsing(JsonConvert.DeserializeObject<ScheduleConstraint>);
                cfg.CreateMap<bool?, string>().ConstructUsing(x => x?.ToString() ?? "");
                cfg.CreateMap<string, bool?>().ConstructUsing(x => bool.TryParse(x, out var parsed) ? parsed : (bool?) null);
                cfg.CreateMap<TimeSpan?, string>().ConstructUsing(x => JsonConvert.SerializeObject(x));
                cfg.CreateMap<string, TimeSpan?>().ConstructUsing(x => TimeSpan.TryParse(x, out var parsed) ? parsed : (TimeSpan?)null);

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