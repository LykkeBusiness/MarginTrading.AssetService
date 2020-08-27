using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class AssetType
    {
        public Guid Id { get; set; }
        public Guid RegulatoryTypeId { get; set; }
        public string Name { get; set; }
    }
}