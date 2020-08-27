using System.Net.Http;
using System.Text;
using Common;

namespace MarginTrading.AssetService.Tests.Extensions
{
    public static class ObjectExtensions
    {
        public static StringContent ToJsonStringContent(this object obj)
        {
            return new StringContent(obj.ToJson(), Encoding.UTF8, "application/json");
        }
    }
}