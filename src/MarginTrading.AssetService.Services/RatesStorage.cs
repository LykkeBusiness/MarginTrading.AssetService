// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Core;
using MarginTrading.AssetService.Core.Domain.Rates;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;

namespace MarginTrading.AssetService.Services
{
    public class RatesStorage : IRatesStorage
    {
        private readonly IMarginTradingBlobRepository _blobRepository;

        public RatesStorage(IMarginTradingBlobRepository blobRepository)
        {
            _blobRepository = blobRepository;
        }

        public async Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRatesAsync()
        {
            var repoData = (await _blobRepository.ReadAsync<IEnumerable<OrderExecutionRate>>(
                blobContainer: LykkeConstants.RateSettingsBlobContainer,
                key: LykkeConstants.OrderExecutionKey
            ))?.ToList();

            return repoData;
        }

        public async Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRatesAsync()
        {
            var repoData = (await _blobRepository.ReadAsync<IEnumerable<OvernightSwapRate>>(
                blobContainer: LykkeConstants.RateSettingsBlobContainer,
                key: LykkeConstants.OvernightSwapKey
            ))?.ToList();

            return repoData;
        }

        public async Task<OnBehalfRate> GetOnBehalfRateAsync()
        {
            var repoData = await _blobRepository.ReadAsync<OnBehalfRate>(
                blobContainer: LykkeConstants.RateSettingsBlobContainer,
                key: LykkeConstants.OnBehalfKey
            );

            return repoData;
        }

        public async Task MergeOrderExecutionRatesAsync(IReadOnlyList<OrderExecutionRate> rates)
        {
            await _blobRepository.MergeListAsync(
                blobContainer: LykkeConstants.RateSettingsBlobContainer,
                key: LykkeConstants.OrderExecutionKey,
                objects: rates.ToList(),
                selector: x => x.AssetPairId);
        }

        public async Task MergeOvernightSwapRatesAsync(IReadOnlyList<OvernightSwapRate> rates)
        {
            await _blobRepository.MergeListAsync(
                blobContainer: LykkeConstants.RateSettingsBlobContainer,
                key: LykkeConstants.OvernightSwapKey,
                objects: rates.ToList(),
                selector: x => x.AssetPairId);
        }

        public async Task ReplaceOnBehalfRateAsync(OnBehalfRate rate)
        {
            await _blobRepository.WriteAsync(
                blobContainer: LykkeConstants.RateSettingsBlobContainer,
                key: LykkeConstants.OnBehalfKey,
                obj: rate);
        }
    }
}