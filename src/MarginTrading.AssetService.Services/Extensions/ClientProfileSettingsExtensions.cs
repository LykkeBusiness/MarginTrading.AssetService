using Lykke.Snow.Common.Percents;
using MarginTrading.AssetService.Core.Domain;
using ClientProfile = MarginTrading.AssetService.Contracts.LegacyAsset.ClientProfile;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class ClientProfileSettingsExtensions
    {
        public static ClientProfile ToClientProfileWithRate(this ClientProfileSettings source, MarginRate marginRate)
        {
            if (source == null)
                return null;
            
            return new ClientProfile
            {
                Id = source.ClientProfileId,
                MarginRate = marginRate,
                ExecutionFeesCap = source.ExecutionFeesCap,
                ExecutionFeesFloor = source.ExecutionFeesFloor,
                ExecutionFeesRate = new ExecutionFeeRate(source.ExecutionFeesRate),
                FinancingFeesRate = new FinancingFeeRate(source.FinancingFeesRate),
                OnBehalfFee = source.OnBehalfFee
            };
        }
    }
}