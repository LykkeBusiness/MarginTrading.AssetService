using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.Common;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.Extensions;
using MarginTrading.SettingsService.Middleware;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Trading instruments management
    /// </summary>
    [Route("api/tradingInstruments")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class TradingInstrumentsController : Controller, ITradingInstrumentsApi
    {
        private readonly IAssetsRepository _assetsRepository;
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly ITradingConditionsRepository _tradingConditionsRepository;
        private readonly ITradingInstrumentsRepository _tradingInstrumentsRepository;
        private readonly ITradingService _tradingService;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        private readonly DefaultTradingInstrumentSettings _defaultTradingInstrumentSettings;
        
        public TradingInstrumentsController(
            IAssetsRepository assetsRepository,
            IAssetPairsRepository assetPairsRepository,
            ITradingConditionsRepository tradingConditionsRepository,
            ITradingInstrumentsRepository tradingInstrumentsRepository,
            ITradingService tradingService,
            IConvertService convertService,
            IEventSender eventSender,
            DefaultTradingInstrumentSettings defaultTradingInstrumentSettings)
        {
            _assetsRepository = assetsRepository;
            _assetPairsRepository = assetPairsRepository;
            _tradingConditionsRepository = tradingConditionsRepository;
            _tradingInstrumentsRepository = tradingInstrumentsRepository;
            _tradingService = tradingService;
            _convertService = convertService;
            _eventSender = eventSender;
            _defaultTradingInstrumentSettings = defaultTradingInstrumentSettings;
        }
        
        /// <summary>
        /// Get the list of trading instruments
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<TradingInstrumentContract>> List([FromQuery] string tradingConditionId)
        {
            var data = string.IsNullOrWhiteSpace(tradingConditionId)
                ? await _tradingInstrumentsRepository.GetAsync()
                : await _tradingInstrumentsRepository.GetByTradingConditionAsync(tradingConditionId);
            
            return data.Select(x => _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of trading instruments with optional pagination
        /// </summary>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<TradingInstrumentContract>> ListByPages(string tradingConditionId, 
            int? skip = null, int? take = null)
        {
            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _tradingInstrumentsRepository.GetByPagesAsync(tradingConditionId, skip, take);
            
            return new PaginatedResponseContract<TradingInstrumentContract>(
                contents: data.Contents.Select(x => _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(x)).ToList(),
                start: data.Start,
                size: data.Size,
                totalSize: data.TotalSize
            );
        }

        /// <summary>
        /// Create new trading instrument
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<TradingInstrumentContract> Insert([FromBody] TradingInstrumentContract instrument)
        {
            await ValidateTradingInstrument(instrument);

            if (!await _tradingInstrumentsRepository.TryInsertAsync(
                _convertService.Convert<TradingInstrumentContract, TradingInstrument>(instrument)))
            {
                throw new ArgumentException($"Trading instrument with tradingConditionId {instrument.TradingConditionId}" +
                                            $"and assetPairId {instrument.Instrument} already exists");
            }

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.TradingInstrument, instrument.Instrument);

            return instrument;
        }

        /// <summary>
        /// Assign trading instrument to a trading condition with default values
        /// </summary>
        [HttpPost]
        [Route("{tradingConditionId}")]
        public async Task<List<TradingInstrumentContract>> AssignCollection(string tradingConditionId, 
            [FromBody] string[] instruments)
        {
            var currentInstruments =
                await _tradingInstrumentsRepository.GetByTradingConditionAsync(tradingConditionId);

            if (currentInstruments.Any())
            {
                var toRemove = currentInstruments.Where(x => !instruments.Contains(x.Instrument)).ToArray();
                
                var existingOrderGroups = await _tradingService.CheckActiveByTradingCondition(tradingConditionId);
                
                if (existingOrderGroups.Any())
                {
                    var errorMessage = existingOrderGroups.Aggregate(
                        "Unable to remove following instruments as they have active orders: ",
                        (current, @group) => current + $"{@group} orders) ");

                    throw new InvalidOperationException(errorMessage);
                }
                
                foreach (var pair in toRemove)
                {
                    await _tradingInstrumentsRepository.DeleteAsync(pair.Instrument, pair.TradingConditionId);
                }
            }
            
            var pairsToAdd = instruments.Where(x => currentInstruments.All(y => y.Instrument != x));

            var addedPairs = await _tradingInstrumentsRepository.CreateDefaultTradingInstruments(tradingConditionId,
                pairsToAdd, _defaultTradingInstrumentSettings);
            
            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingInstrument);

            return addedPairs.Select(x => _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(x))
                .ToList();
        }

        /// <summary>
        /// Get trading instrument
        /// </summary>
        [HttpGet]
        [Route("{tradingConditionId}/{assetPairId}")]
        public async Task<TradingInstrumentContract> Get(string tradingConditionId, string assetPairId)
        {
            var obj = await _tradingInstrumentsRepository.GetAsync(assetPairId, tradingConditionId);

            return _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(obj);
        }

        /// <summary>
        /// Update the trading instrument
        /// </summary>
        [HttpPut]
        [Route("{tradingConditionId}/{assetPairId}")]
        public async Task<TradingInstrumentContract> Update(string tradingConditionId, string assetPairId, 
            [FromBody] TradingInstrumentContract instrument)
        {
            await ValidateTradingInstrument(instrument);
            ValidateId(tradingConditionId, assetPairId, instrument);

            await _tradingInstrumentsRepository.UpdateAsync(
                _convertService.Convert<TradingInstrumentContract, TradingInstrument>(instrument));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.TradingInstrument, instrument.Instrument);
            
            return instrument;
        }

        /// <summary>
        /// Update list of trading instruments
        /// </summary>
        [HttpPut]
        [Route("{tradingConditionId}/batch")]
        public async Task<List<TradingInstrumentContract>> UpdateList(string tradingConditionId,
            [FromBody] TradingInstrumentContract[] instruments)
        {
            foreach (var instrument in instruments)
            {
                await ValidateTradingInstrument(instrument);
                ValidateId(tradingConditionId, instrument.Instrument, instrument);
            }

            var itemsToUpdate = instruments
                .Select(i => _convertService.Convert<TradingInstrumentContract, TradingInstrument>(i))
                .ToList();
            var updated = await _tradingInstrumentsRepository.UpdateBatchAsync(tradingConditionId, itemsToUpdate);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingInstrument);

            return updated.Select(x => _convertService.Convert<ITradingInstrument, TradingInstrumentContract>(x))
                .ToList();
        }

        /// <summary>
        /// Delete the trading instrument
        /// </summary>
        [HttpDelete]
        [Route("{tradingConditionId}/{assetPairId}")]
        public async Task Delete(string tradingConditionId, string assetPairId)
        {
            await _tradingInstrumentsRepository.DeleteAsync(assetPairId, tradingConditionId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.TradingInstrument, assetPairId);
        }

        private void ValidateId(string tradingConditionId, string assetPairId, TradingInstrumentContract contract)
        {
            if (contract?.TradingConditionId != tradingConditionId)
            {
                throw new ArgumentException("TradingConditionId must match with contract tradingConditionId");
            }

            if (contract?.Instrument != assetPairId)
            {
                throw new ArgumentException("AssetPairId must match with contract instrument");
            }
        }

        private async Task ValidateTradingInstrument(TradingInstrumentContract instrument)
        {
            if (instrument == null)
            {
                throw new ArgumentNullException("instrument", "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(instrument?.TradingConditionId))
            {
                throw new ArgumentNullException(nameof(instrument.TradingConditionId), "TradingConditionId must be set");
            }
            
            if (string.IsNullOrWhiteSpace(instrument.Instrument))
            {
                throw new ArgumentNullException(nameof(instrument.Instrument), "Instrument must be set");
            }

            if (await _tradingConditionsRepository.GetAsync(instrument.TradingConditionId) == null)
            {
                throw new InvalidOperationException($"Trading condition {instrument.TradingConditionId} does not exist");
            }

            if (await _assetPairsRepository.GetAsync(instrument.Instrument) == null)
            {
                throw new InvalidOperationException($"Asset pair {instrument.Instrument} does not exist");
            }

            if (instrument.LeverageInit <= 0)
            {
                throw new InvalidOperationException($"LeverageInit must be greather then zero");
            }

            if (instrument.LeverageMaintenance <= 0)
            {
                throw new InvalidOperationException($"LeverageMaintenance must be greather then zero");
            }

            if (await _assetsRepository.GetAsync(instrument.CommissionCurrency) == null)
            {
                throw new InvalidOperationException($"Commission currency {instrument.CommissionCurrency} does not exist");
            }
        }
    }
}