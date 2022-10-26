using System;
using AutoMapper;
using JetBrains.Annotations;

namespace MarginTrading.AssetService.Services.Mapping
{
    [UsedImplicitly]
    internal sealed class NullTimespanTypeConverter : ITypeConverter<string, TimeSpan?>
    {
        public TimeSpan? Convert(string source, TimeSpan? destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            
            if (TimeSpan.TryParse(source, out TimeSpan result))
                return result;
            
            return null;
        }
    }
}