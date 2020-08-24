using System;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class ClientProfileEntity
    {
        public Guid Id { get; set; }
        public Guid RegulatoryProfileId { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public bool IsDefault { get; set; }
    }
}