using MarginTrading.AssetService.Services.Mapping;

using Xunit;

namespace MarginTrading.AssetService.Tests.UnitTests
{
    public class MappingTests
    {
        [Fact]
        public void ShouldHaveValidMappingConfiguration()
        {
            var convertService = new ConvertService();
            
            convertService.AssertConfigurationIsValid();
        }
    }
}