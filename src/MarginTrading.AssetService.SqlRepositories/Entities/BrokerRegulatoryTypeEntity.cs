using System;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class BrokerRegulatoryTypeEntity
    {
        public Guid Id { get; set; }
        public Guid RegulatoryTypeId { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}