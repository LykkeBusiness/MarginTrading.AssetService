// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;

using MarginTrading.AssetService.Core.Exceptions;

namespace MarginTrading.AssetService.Services.Extensions
{
    internal static class BrokerSettingsApiExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="brokerSettingsApi"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BrokerSettingsDoNotExistException"></exception>
        public static async Task<BrokerSettingsContract> GetByIdOrThrowAsync(this IBrokerSettingsApi brokerSettingsApi,
            string id)
        {
            var response = await brokerSettingsApi.GetByIdAsync(id);
            if (response.ErrorCode == BrokerSettingsErrorCodesContract.BrokerSettingsDoNotExist)
            {
                throw new BrokerSettingsDoNotExistException();
            }

            return response.BrokerSettings;
        }
        
        public static async Task<string> GetRegulationIdOrThrowAsync(this IBrokerSettingsApi brokerSettingsApi,
            string id)
        {
            var response = await brokerSettingsApi.GetByIdOrThrowAsync(id);
            return response.RegulationId;
        }
        
        public static async Task<string> GetSettlementCurrencyOrThrowAsync(this IBrokerSettingsApi brokerSettingsApi,
            string id)
        {
            var response = await brokerSettingsApi.GetByIdOrThrowAsync(id);
            return response.SettlementCurrency;
        }
    }
}