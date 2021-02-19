using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class ClientProfileSettingsEntity
    {
        public string ClientProfileId { get; set; }
        public ClientProfileEntity ClientProfile { get; set; }
        public string AssetTypeId { get; set; }
        public AssetTypeEntity AssetType { get; set; }
        public decimal Margin { get; set; }
        public decimal ExecutionFeesFloor { get; set; }
        public decimal ExecutionFeesCap { get; set; }
        public decimal ExecutionFeesRate { get; set; }
        public decimal FinancingFeesRate { get; set; }
        public decimal OnBehalfFee { get; set; }
        public bool IsAvailable { get; set; }

        public static ClientProfileSettingsEntity Create(ClientProfileSettings model)
        {
            return new ClientProfileSettingsEntity
            {
                ExecutionFeesFloor = model.ExecutionFeesFloor,
                IsAvailable = model.IsAvailable,
                Margin = model.Margin,
                ExecutionFeesRate = model.ExecutionFeesRate,
                ExecutionFeesCap = model.ExecutionFeesCap,
                OnBehalfFee = model.OnBehalfFee,
                AssetTypeId = model.AssetTypeId,
                ClientProfileId = model.ClientProfileId,
                FinancingFeesRate = model.FinancingFeesRate,
            };
        }
    }
}