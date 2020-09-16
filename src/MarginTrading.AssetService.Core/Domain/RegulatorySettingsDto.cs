namespace MarginTrading.AssetService.Core.Domain
{
    public class RegulatorySettingsDto
    {
        public string RegulatoryProfileId { get; set; }

        public string RegulatoryTypeId { get; set; }

        public decimal MarginMin { get; set; }

        public bool IsAvailable { get; set; }
    }
}