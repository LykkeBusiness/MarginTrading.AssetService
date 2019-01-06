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
using MarginTrading.SettingsService.Middleware;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Asset pairs management
    /// </summary>
    [Route("api/assetPairs")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
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
        /// <param name="filter">Search by Id and Name</param>
        [HttpGet]
        [Route("")]
        public async Task<List<AssetPairContract>> List([FromQuery] string legalEntity = null,
            [FromQuery] MatchingEngineModeContract? matchingEngineMode = null, string filter = null)
        {
            var data = await _assetPairsRepository.GetByLeAndMeModeAsync(legalEntity, matchingEngineMode?.ToString(), 
                filter);
            
            return data.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of asset pairs based on legal entity and matching engine mode, with optional pagination
        /// </summary>
        /// <param name="legalEntity"></param>
        /// <param name="matchingEngineMode"></param>
        /// <param name="filter">Search by Id and Name</param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<AssetPairContract>> ListByPages([FromQuery] string legalEntity = null, 
            [FromQuery] MatchingEngineModeContract? matchingEngineMode = null, [FromQuery] string filter = null,
            [FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _assetPairsRepository.GetByLeAndMeModeByPagesAsync(legalEntity, 
                matchingEngineMode?.ToString(), filter, skip, take);
            
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
            await ValidatePairInsert(assetPair);
            
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
                await ValidatePairInsert(assetPair);
            
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
        public async Task<AssetPairContract> Update(string assetPairId, 
            [FromBody] AssetPairUpdateRequest assetPairUpdateRequest)
        {
            await ValidatePairUpdate(assetPairUpdateRequest);
            ValidateId(assetPairId, assetPairUpdateRequest.Id);

            var updated = await _assetPairsRepository.UpdateAsync(assetPairUpdateRequest);
            
            if (updated == null)
            {
                throw new ArgumentException("Update failed", nameof(assetPairUpdateRequest));
            }
            
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
        public async Task<List<AssetPairContract>> BatchUpdate(
            [FromBody] AssetPairUpdateRequest[] assetPairsUpdateRequest)
        {
            foreach (var assetPairUpdateRequest in assetPairsUpdateRequest)
            {
                await ValidatePairUpdate(assetPairUpdateRequest);
            }
            await ValidateUnique(assetPairsUpdateRequest);

            var updated = await _assetPairsRepository.UpdateBatchAsync(assetPairsUpdateRequest.ToList());
            
            if (updated == null)
            {
                throw new ArgumentException("Batch update failed", nameof(assetPairsUpdateRequest));
            }
            
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
            await _cqrsMessageSender.SendAssetPairChangedEvent(new AssetPairChangedEvent
            {
                OperationId = Guid.NewGuid().ToString("N"),
                AssetPair = new AssetPairContract { Id = assetPairId },
            });
        }

        private async Task ValidatePairInsert(AssetPairContract assetPair)
        {
            if (assetPair == null)
            {
                throw new ArgumentNullException(nameof(assetPair), "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(assetPair.Id))
            {
                throw new ArgumentNullException(nameof(assetPair.Id), "AssetPair Id must be set");
            }

            if (!Enum.IsDefined(typeof(MatchingEngineModeContract), assetPair.MatchingEngineMode))
            {
                throw new ArgumentNullException(nameof(assetPair.MatchingEngineMode), "AssetPair MatchingEngineMode must be set");
            }

            if (await _assetsRepository.GetAsync(assetPair.BaseAssetId) == null)
            {
                throw new InvalidOperationException($"Base Asset {assetPair.BaseAssetId} does not exist");
            }

            if (await _assetsRepository.GetAsync(assetPair.QuoteAssetId) == null)
            {
                throw new InvalidOperationException($"Quote Asset {assetPair.QuoteAssetId} does not exist");
            }

            if (!string.IsNullOrEmpty(assetPair.MarketId)
                && await _marketRepository.GetAsync(assetPair.MarketId) == null)
            {
                throw new InvalidOperationException($"Market {assetPair.MarketId} does not exist");
            }

            if (assetPair.StpMultiplierMarkupAsk <= 0)
            {
                throw new InvalidOperationException($"StpMultiplierMarkupAsk must be greater then zero");
            }
            
            if (assetPair.StpMultiplierMarkupBid <= 0)
            {
                throw new InvalidOperationException($"StpMultiplierMarkupBid must be greater then zero");
            }

            if (await _assetPairsRepository.GetByBaseQuoteAndLegalEntityAsync(assetPair.BaseAssetId, 
                assetPair.QuoteAssetId, assetPair.LegalEntity) != null)
            {
                throw new InvalidOperationException($"Asset pair with base asset [{assetPair.BaseAssetId}], quote asset [{assetPair.QuoteAssetId}] and legal entity [{assetPair.LegalEntity}] already exists");
            }
            
            //base pair check <-- the last one
            if (assetPair.BasePairId == null) 
                return;

            if (await _assetPairsRepository.GetAsync(assetPair.BasePairId) == null)
            {
                throw new InvalidOperationException($"BasePair with Id {assetPair.BasePairId} does not exist");
            }

            if (await _assetPairsRepository.GetByBaseAssetPairAsync(assetPair.BasePairId) != null)
            {
                throw new InvalidOperationException($"BasePairId {assetPair.BasePairId} does not exist");
            }

            if (await _assetPairsRepository.GetByBaseAssetPairAndNotByIdAsync(assetPair.Id, assetPair.BasePairId) != null)
            {
                throw new InvalidOperationException($"BasePairId {assetPair.BasePairId} cannot be added twice");
            }

            if (await _assetPairsRepository.GetByBaseQuoteAndLegalEntityAsync(assetPair.BaseAssetId,
                    assetPair.QuoteAssetId, assetPair.LegalEntity) != null)
            {
                throw new InvalidOperationException($"Asset pair with base: {assetPair.BaseAssetId}, quote: {assetPair.QuoteAssetId}, legalEntity: {assetPair.LegalEntity} already exists");   
            }
        }

        private async Task ValidatePairUpdate(AssetPairUpdateRequest assetPair)
        {
            if (assetPair == null)
            {
                throw new ArgumentNullException(nameof(assetPair), "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(assetPair.Id))
            {
                throw new ArgumentNullException(nameof(assetPair.Id), "AssetPair Id must be set");
            }

            if (assetPair.MatchingEngineMode != null
                && !Enum.IsDefined(typeof(MatchingEngineModeContract), assetPair.MatchingEngineMode))
            {
                throw new ArgumentNullException(nameof(assetPair.MatchingEngineMode), "AssetPair MatchingEngineMode must be set");
            }

            if (assetPair.BaseAssetId != null
                && await _assetsRepository.GetAsync(assetPair.BaseAssetId) == null)
            {
                throw new InvalidOperationException($"Base Asset {assetPair.BaseAssetId} does not exist");
            }

            if (assetPair.QuoteAssetId != null
                && await _assetsRepository.GetAsync(assetPair.QuoteAssetId) == null)
            {
                throw new InvalidOperationException($"Quote Asset {assetPair.QuoteAssetId} does not exist");
            }

            if (!string.IsNullOrEmpty(assetPair.MarketId)
                && await _marketRepository.GetAsync(assetPair.MarketId) == null)
            {
                throw new InvalidOperationException($"Market {assetPair.MarketId} does not exist");
            }

            if (assetPair.StpMultiplierMarkupAsk != null
                && assetPair.StpMultiplierMarkupAsk <= 0)
            {
                throw new InvalidOperationException($"StpMultiplierMarkupAsk must be greater then zero");
            }
            
            if (assetPair.StpMultiplierMarkupBid != null
                && assetPair.StpMultiplierMarkupBid <= 0)
            {
                throw new InvalidOperationException($"StpMultiplierMarkupBid must be greater then zero");
            }
            
            //base pair check <-- the last one
            if (assetPair.BasePairId == null) 
                return;

            if (await _assetPairsRepository.GetAsync(assetPair.BasePairId) == null)
            {
                throw new InvalidOperationException($"BasePair with Id {assetPair.BasePairId} does not exist");
            }

            if (await _assetPairsRepository.GetByBaseAssetPairAsync(assetPair.BasePairId) != null)
            {
                throw new InvalidOperationException($"BasePairId {assetPair.BasePairId} does not exist");
            }

            if (await _assetPairsRepository.GetByBaseAssetPairAndNotByIdAsync(assetPair.Id, assetPair.BasePairId) != null)
            {
                throw new InvalidOperationException($"BasePairId {assetPair.BasePairId} cannot be added twice");
            }

            if (await _assetPairsRepository.GetByBaseQuoteAndLegalEntityAsync(assetPair.BaseAssetId,
                    assetPair.QuoteAssetId, assetPair.LegalEntity) != null)
            {
                throw new InvalidOperationException($"Asset pair with base: {assetPair.BaseAssetId}, quote: {assetPair.QuoteAssetId}, legalEntity: {assetPair.LegalEntity} already exists");   
            }
        }

        private void ValidateId(string id, string contractId)
        {
            if (contractId != id)
            {
                throw new ArgumentException("Id must match with contract id");
            }
        }

        private void ValidateUnique(AssetPairContract[] assetPairs)
        {
            ValidateUnique(assetPairs.Select(x => (x.BaseAssetId, x.QuoteAssetId, x.LegalEntity)).ToList());
        }

        private async Task ValidateUnique(AssetPairUpdateRequest[] assetPairUpdateRequests)
        {
            var current = (await _assetPairsRepository.GetAsync(assetPairUpdateRequests.Select(x => x.Id).ToArray()))
                .ToDictionary(x => x.Id, x => x);
            ValidateUnique(assetPairUpdateRequests.Select(x => 
                (
                    x.BaseAssetId ?? current[x.Id].BaseAssetId,
                    x.QuoteAssetId ?? current[x.Id].QuoteAssetId,
                    x.LegalEntity ?? current[x.Id].LegalEntity
                )).ToList());
        }
        
        private void ValidateUnique(List<(string BaseAssetId, string QuoteAssetId, string LegalEntity)> assetPairs)
        {
            var groups = assetPairs.GroupBy(x => (x.BaseAssetId, x.QuoteAssetId, x.LegalEntity))
                .Where(x => x.Count() > 1).ToList();
            if (assetPairs.Count == 0 || groups.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(assetPairs), 
                    $"Only unique asset pairs are allowed. The list of non-unique: {string.Join(", ", groups.Select(x => $"({x.Key.BaseAssetId},{x.Key.QuoteAssetId},{x.Key.LegalEntity})"))}");
            }
        }
    }
}