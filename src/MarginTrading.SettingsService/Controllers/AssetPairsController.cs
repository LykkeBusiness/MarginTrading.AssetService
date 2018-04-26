using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Client;
using MarginTrading.SettingsService.Client.AssetPair;
using MarginTrading.SettingsService.Client.Enums;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
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
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public AssetPairsController(
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService, 
            IEventSender eventSender)
        {
            _assetPairsRepository = assetPairsRepository;
            _convertService = convertService;
            _eventSender = eventSender;
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
            Enum.TryParse<MatchingEngineMode>(matchingEngineMode?.ToString(), out var matchingEngineModeDomain);

            var data = string.IsNullOrEmpty(legalEntity) && matchingEngineMode == null
                ? await _assetPairsRepository.GetAsync()
                : await _assetPairsRepository.GetAsync(assetPair =>
                    (string.IsNullOrEmpty(legalEntity) || assetPair.LegalEntity == legalEntity)
                    && (matchingEngineMode == null ||
                        ((AssetPair) assetPair).MatchingEngineMode == matchingEngineModeDomain));
            
            return data.Select(x => _convertService.Convert<IAssetPair, AssetPairContract>(x)).ToList();
        }

        /// <summary>
        /// Create new asset pair
        /// </summary>
        /// <param name="assetPair"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<AssetPairContract> Insert([FromBody] AssetPairContract assetPair)
        {
            if (string.IsNullOrWhiteSpace(assetPair?.Id))
            {
                throw new ArgumentNullException(nameof(assetPair.Id), "assetPair Id must be set");
            }
            
            await _assetPairsRepository.InsertAsync(_convertService.Convert<AssetPairContract, AssetPair>(assetPair));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
            
            return assetPair;
        }

        /// <summary>
        /// Get asset pair by id
        /// </summary>
        /// <param name="assetPairId"></param>
        /// <returns></returns>
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
        /// <param name="assetPairId"></param>
        /// <param name="assetPair"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{assetPairId}")]
        public async Task<AssetPairContract> Update(string assetPairId, [FromBody] AssetPairContract assetPair)
        {
            if (string.IsNullOrWhiteSpace(assetPair?.Id))
            {
                throw new ArgumentNullException(nameof(assetPair.Id), "assetPair Id must be set");
            }

            await _assetPairsRepository.ReplaceAsync(_convertService.Convert<AssetPairContract, AssetPair>(assetPair));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
            
            return assetPair;
        }

        /// <summary>
        /// Delete asset pair
        /// </summary>
        /// <param name="assetPairId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{assetPairId}")]
        public async Task Delete(string assetPairId)
        {
            await _assetPairsRepository.DeleteAsync(assetPairId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.AssetPair);
        }
    }
}