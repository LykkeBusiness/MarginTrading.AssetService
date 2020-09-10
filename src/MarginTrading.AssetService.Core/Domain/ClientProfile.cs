using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ClientProfile
    {
        public string Id { get; set; }
        public string RegulatoryProfileId { get; set; }
        public bool IsDefault { get; set; }
    }
}