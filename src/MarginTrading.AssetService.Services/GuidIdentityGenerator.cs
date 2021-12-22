using System;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services
{
    public class GuidIdentityGenerator : IIdentityGenerator
    {
        public string GenerateId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}