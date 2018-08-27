using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Contracts.Common;
using MarginTrading.SettingsService.Contracts.Enums;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
using MarginTrading.SettingsService.Extensions;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Asset pairs management
    /// </summary>
    [Route("api/assetPairs")]
    public class AssetPairsController : Controller, IAssetPairsApi
    {
        private readonly IAssetsRepository _assetsRepository;
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IMarketRepository _marketRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        private readonly ICqrsMessageSender _cqrsMessageSender;
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;
        
        public AssetPairsController(
            IAssetsRepository assetsRepository,
            IAssetPairsRepository assetPairsRepository,
            IMarketRepository marketRepository,
            IConvertService convertService, 
            IEventSender eventSender,
            ICqrsMessageSender cqrsMessageSender,
            DefaultLegalEntitySettings defaultLegalEntitySettings)
        {
            _assetsRepository = assetsRepository;
            _assetPairsRepository = assetPairsRepository;
            _marketRepository = marketRepository;
            _convertService = convertService;
            _eventSender = eventSender;
            _cqrsMessageSender = cqrsMessageSender;
            _defaultLegalEntitySettings = defaultLegalEntitySettings;
        }
        
        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode
        /// </summary>
        /// <param name="legalEntity"></param>
        /// <param name="matchingEngineMode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<List<AssetPairContract>> List([FromQuery] string legalEntity = null, 
            [FromQuery] MatchingEngineModeContract? matchingEngineMode = null)
        {
            var data = await _assetPairsRepository.GetByLeAndMeModeAsync(legalEntity, matchingEngineMode?.ToString());
            
            return data.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode, with optional pagination
        /// </summary>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<AssetPairContract>> ListByPages([FromQuery] string legalEntity = null, 
            [FromQuery] MatchingEngineModeContract? matchingEngineMode = null, 
            [FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _assetPairsRepository.GetByLeAndMeModeByPagesAsync(legalEntity, 
                matchingEngineMode?.ToString(), skip, take);
            
            return new PaginatedResponseContract<AssetPairContract>(
                contents: data.Contents.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList(),
                start: data.Start,
                size: data.Size,
                totalSize: data.TotalSize
            );
        }

        /// <summary>
        /// Create new asset pair
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<AssetPairContract> Insert([FromBody] AssetPairContract assetPair)
        {
            await ValidatePair(assetPair);
            
            _defaultLegalEntitySettings.Set(assetPair);

            var inserted = await _assetPairsRepository.InsertAsync(
                _convertService.Convert<AssetPairContract, AssetPair>(assetPair));
            if (inserted == null)
            {
                throw new ArgumentException($"Asset pair with id {assetPair.Id} already exists", nameof(assetPair.Id));
            }

            var insertedContract = _convertService.Convert<IAssetPair, AssetPairContract>(inserted);
            
            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
            await _cqrsMessageSender.SendAssetPairChangedEvent(new AssetPairChangedEvent
            {
                OperationId = Guid.NewGuid().ToString("N"),
                AssetPair = insertedContract,
            });
            
            return insertedContract;
        }

        /// <summary>
        /// Create new asset pairs in a batch request
        /// </summary>
        [HttpPost]
        [Route("batch")]
        public async Task<List<AssetPairContract>> BatchInsert([FromBody] AssetPairContract[] assetPairs)
        {
            foreach (var assetPair in assetPairs)
            {
                await ValidatePair(assetPair);
            
                _defaultLegalEntitySettings.Set(assetPair);   
            }
            ValidateUnique(assetPairs);

            var inserted = await _assetPairsRepository.InsertBatchAsync(assetPairs.Select(x =>
                _convertService.Convert<AssetPairContract, AssetPair>(x)).ToList()); 
                
            if (inserted == null)
            {
                throw new ArgumentException("One of asset pairs already exist", nameof(assetPairs));
            }

            var insertedContracts = inserted.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();
            
            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
            foreach (var insertedContract in insertedContracts)
            {
                await _cqrsMessageSender.SendAssetPairChangedEvent(new AssetPairChangedEvent
                {
                    OperationId = Guid.NewGuid().ToString("N"),
                    AssetPair = insertedContract,
                });
            }

            return insertedContracts;
        }

        /// <summary>
        /// Get asset pair by id
        /// </summary>
        [HttpGet]
        [Route("{assetPairId}")]
        public async Task<AssetPairContract> Get(string assetPairId)
        {
            var obj = await _assetPairsRepository.GetAsync(assetPairId);
            return _convertService.Convert<IAssetPair, AssetPairContract>(obj);
        }

        /// <summary>
        /// Update asset pair
        /// </summary>
        [HttpPut]
        [Route("{assetPairId}")]
        public async Task<AssetPairContract> Update(string assetPairId, [FromBody] AssetPairContract assetPair)
        {
            await ValidatePair(assetPair);
            ValidateId(assetPairId, assetPair);

            _defaultLegalEntitySettings.Set(assetPair);

            var updated = await _assetPairsRepository.UpdateAsync(_convertService.Convert<AssetPairContract, AssetPair>(assetPair));
            var updatedContract = _convertService.Convert<IAssetPair, AssetPairContract>(updated);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
            await _cqrsMessageSender.SendAssetPairChangedEvent(new AssetPairChangedEvent
            {
                OperationId = Guid.NewGuid().ToString("N"),
                AssetPair = updatedContract,
            });
            
            return updatedContract;
        }

        /// <summary>
        /// Update asset pairs in a batch request
        /// </summary>
        [HttpPut]
        [Route("batch")]
        public async Task<List<AssetPairContract>> BatchUpdate([FromBody] AssetPairContract[] assetPairs)
        {
            foreach (var assetPair in assetPairs)
            {
                await ValidatePair(assetPair);

                _defaultLegalEntitySettings.Set(assetPair);
            }
            ValidateUnique(assetPairs);

            var updated = await _assetPairsRepository.UpdateBatchAsync(assetPairs.Select(x => 
                _convertService.Convert<AssetPairContract, AssetPair>(x)).ToList());
            var updatedContracts = updated.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
            foreach (var updatedContract in updatedContracts)
            {
                await _cqrsMessageSender.SendAssetPairChangedEvent(new AssetPairChangedEvent
                {
                    OperationId = Guid.NewGuid().ToString("N"),
                    AssetPair = updatedContract,
                });
            }

            return updatedContracts;
        }

        /// <summary>
        /// Delete asset pair
        /// </summary>
        [HttpDelete]
        [Route("{assetPairId}")]
        public async Task Delete(string assetPairId)
        {
            await _assetPairsRepository.DeleteAsync(assetPairId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
        }

        private async Task ValidatePair(AssetPairContract newValue)
        {
            if (newValue == null)
            {
                throw new ArgumentNullException("assetPair", "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(newValue.Id))
            {
                throw new ArgumentNullException(nameof(newValue.Id), "AssetPair Id must be set");
            }

            if (!Enum.IsDefined(typeof(MatchingEngineModeContract), newValue.MatchingEngineMode))
            {
                throw new ArgumentNullException(nameof(newValue.MatchingEngineMode), "AssetPair MatchingEngineMode must be set");
            }

            if (await _assetsRepository.GetAsync(newValue.BaseAssetId) == null)
            {
                throw new InvalidOperationException($"Base Asset {newValue.BaseAssetId} does not exist");
            }

            if (await _assetsRepository.GetAsync(newValue.QuoteAssetId) == null)
            {
                throw new InvalidOperationException($"Quote Asset {newValue.QuoteAssetId} does not exist");
            }

            if (!string.IsNullOrEmpty(newValue.MarketId)
                && await _marketRepository.GetAsync(newValue.MarketId) == null)
            {
                throw new InvalidOperationException($"Market {newValue.MarketId} does not exist");
            }

            if (newValue.StpMultiplierMarkupAsk <= 0)
            {
                throw new InvalidOperationException($"StpMultiplierMarkupAsk must be greather then zero");
            }
            
            if (newValue.StpMultiplierMarkupBid <= 0)
            {
                throw new InvalidOperationException($"StpMultiplierMarkupBid must be greather then zero");
            }
            
            //base pair check <-- the last one
            if (newValue.BasePairId == null) 
                return;

            if (await _assetPairsRepository.GetAsync(newValue.BasePairId) == null)
            {
                throw new InvalidOperationException($"BasePair with Id {newValue.BasePairId} does not exist");
            }

            if (await _assetPairsRepository.GetByBaseAssetPairAsync(newValue.BasePairId) != null)
            {
                throw new InvalidOperationException($"BasePairId {newValue.BasePairId} does not exist");
            }

            if (await _assetPairsRepository.GetByBaseAssetPairAndNotByIdAsync(newValue.Id, newValue.BasePairId) != null)
            {
                throw new InvalidOperationException($"BasePairId {newValue.BasePairId} cannot be added twice");
            }

            if (await _assetPairsRepository.GetByBaseQuoteAndLegalEntityAsync(newValue.BaseAssetId,
                    newValue.QuoteAssetId, newValue.LegalEntity) != null)
            {
                throw new InvalidOperationException($"Asset pair with base: {newValue.BaseAssetId}, quote: {newValue.QuoteAssetId}, legalEntity: {newValue.LegalEntity} already exists");   
            }
        }

        private void ValidateId(string id, AssetPairContract contract)
        {
            if (contract?.Id != id)
            {
                throw new ArgumentException("Id must match with contract id");
            }
        }

        private void ValidateUnique(AssetPairContract[] assetPairs)
        {
            if (assetPairs.Length == 0 || assetPairs.Select(x => x.Id).Count() != assetPairs.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(assetPairs), "Only unique asset pairs are allowed.");
            }
        }
    }
}