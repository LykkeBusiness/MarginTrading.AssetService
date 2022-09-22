using AutoMapper;
using JetBrains.Annotations;

namespace MarginTrading.AssetService.Services.Mapping
{
    [UsedImplicitly]
    internal sealed class NullBooleanTypeConverter : ITypeConverter<string, bool?>
    {
        public bool? Convert(string source, bool? destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            
            if (bool.TryParse(source, out bool result))
                return result;
            
            return null;
        }
    }
}