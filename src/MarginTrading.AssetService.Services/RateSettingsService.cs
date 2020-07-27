// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using MarginTrading.AssetService.Core.Domain.Rates;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings.Rates;


namespace MarginTrading.AssetService.Services
{
    public class RateSettingsService : IRateSettingsService
    {
        private readonly IRatesRepository _ratesRepository;

        private readonly ILog _log;
        private readonly DefaultRateSettings _defaultRateSettings;

        public RateSettingsService(
            IRatesRepository ratesRepository,
            ILog log,
            DefaultRateSettings defaultRateSettings)
        {
            _ratesRepository = ratesRepository;
            _log = log;
            _defaultRateSettings = defaultRateSettings;
        }

        #region Order Execution

        public async Task<IReadOnlyList<OrderExecutionRate>> GetOrderExecutionRates(IList<string> assetPairIds = null)
        {
            var repoData = await _ratesRepository.GetOrderExecutionRatesAsync();
            
            if (assetPairIds == null || !assetPairIds.Any())
                return repoData.ToList();

            return assetPairIds
                .Select(assetPairId => GetOrderExecutionRateSingleOrDefault(assetPairId, repoData))
                .ToList();
        }

        private OrderExecutionRate GetOrderExecutionRateSingleOrDefault(string assetPairId,
            IReadOnlyCollection<OrderExecutionRate> repoData)
        {
            var rate = repoData?.FirstOrDefault(x => x.AssetPairId == assetPairId);
            if (rate == null)
            {
                _log.WriteWarning(nameof(RateSettingsService), nameof(GetOrderExecutionRateSingleOrDefault),
                    $"No order execution rate for {assetPairId}. Using the default one.");

                var rateFromDefault =
                    OrderExecutionRate.FromDefault(_defaultRateSettings.DefaultOrderExecutionSettings, assetPairId);

                return rateFromDefault;
            }

            return rate;
        }

        public async Task ReplaceOrderExecutionRates(List<OrderExecutionRate> rates)
        {
            rates = rates.Select(x =>
            {
                if (string.IsNullOrWhiteSpace(x.LegalEntity))
                {
                    x.LegalEntity = _defaultRateSettings.DefaultOrderExecutionSettings.LegalEntity;
                }

                return x;
            }).ToList();

            await _ratesRepository.MergeOrderExecutionRatesAsync(rates);
        }

        #endregion Order Execution

        #region Overnight Swaps

        public async Task<IReadOnlyList<OvernightSwapRate>> GetOvernightSwapRates()
        {
            return await _ratesRepository.GetOvernightSwapRatesAsync();
        }

        public async Task ReplaceOvernightSwapRates(List<OvernightSwapRate> rates)
        {
            await _ratesRepository.MergeOvernightSwapRatesAsync(rates);
        }

        #endregion Overnight Swaps

        #region On Behalf

        public async Task<OnBehalfRate> GetOnBehalfRate()
        {
            var rate = await _ratesRepository.GetOnBehalfRateAsync();
            if (rate == null)
            {
                await _log.WriteWarningAsync(nameof(RateSettingsService), nameof(GetOnBehalfRate),
                    $"No OnBehalf rate saved, using the default one.");

                rate = OnBehalfRate.FromDefault(_defaultRateSettings.DefaultOnBehalfSettings);
            }

            return rate;
        }

        public async Task ReplaceOnBehalfRate(OnBehalfRate rate)
        {
            if (string.IsNullOrWhiteSpace(rate.LegalEntity))
            {
                rate.LegalEntity = _defaultRateSettings.DefaultOrderExecutionSettings.LegalEntity;
            }

            await _ratesRepository.ReplaceOnBehalfRateAsync(rate);
        }

        #endregion On Behalf
    }
}