using System;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class ClientProfileEntity
    {
        public string Id { get; set; }
        public string RegulatoryProfileId { get; set; }
        public bool IsDefault { get; set; }
    }
}