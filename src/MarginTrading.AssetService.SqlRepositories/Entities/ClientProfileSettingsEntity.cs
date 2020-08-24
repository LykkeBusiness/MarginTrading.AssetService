using System;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.SqlRepositories.Entities
{
    public class ClientProfileSettingsEntity
    {
        public Guid ClientProfileId { get; set; }
        public ClientProfileEntity ClientProfile { get; set; }
        public Guid AssetTypeId { get; set; }
        public AssetTypeEntity AssetType { get; set; }
        public decimal MarginMin { get; set; }
        public decimal ExecutionFeesFloor { get; set; }
        public decimal ExecutionFeesCap { get; set; }
        public decimal ExecutionFeesRate { get; set; }
        public decimal FinancingFeesRate { get; set; }
        public decimal PhoneFees { get; set; }
        public bool IsAvailable { get; set; }

        public static ClientProfileSettingsEntity Create(ClientProfileSettings model)
        {
            return new ClientProfileSettingsEntity
            {
                ExecutionFeesFloor = model.ExecutionFeesFloor,
                IsAvailable = model.IsAvailable,
                MarginMin = model.MarginMin,
                ExecutionFeesRate = model.ExecutionFeesRate,
                ExecutionFeesCap = model.ExecutionFeesCap,
                PhoneFees = model.PhoneFees,
                AssetTypeId = model.AssetTypeId,
                ClientProfileId = model.ClientProfileId,
                FinancingFeesRate = model.FinancingFeesRate,
            };
        }
    }
}