﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Contracts.Enums;
using MarginTrading.SettingsService.Core;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
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
        private readonly DefaultLegalEntitySettings _defaultLegalEntitySettings;
        
        public AssetPairsController(
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService, 
            IEventSender eventSender,
            DefaultLegalEntitySettings defaultLegalEntitySettings)
        {
            _assetPairsRepository = assetPairsRepository;
            _convertService = convertService;
            _eventSender = eventSender;
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
            Enum.TryParse<MatchingEngineMode>(matchingEngineMode?.ToString(), out var matchingEngineModeDomain);

            var data = string.IsNullOrEmpty(legalEntity) && matchingEngineMode == null
                ? await _assetPairsRepository.GetAsync()
                : await _assetPairsRepository.GetAsync(assetPair =>
                    (string.IsNullOrEmpty(legalEntity) || assetPair.LegalEntity == legalEntity)
                    && (matchingEngineMode == null ||
                        assetPair.MatchingEngineMode == matchingEngineModeDomain));
            
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
            await ValidatePair(assetPair);

            _defaultLegalEntitySettings.Set(assetPair);
            
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
            ValidateId(assetPairId, assetPair);

            await ValidatePair(assetPair);

            _defaultLegalEntitySettings.Set(assetPair);

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

        private async Task ValidatePair(AssetPairContract newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue?.Id))
            {
                throw new ArgumentNullException(nameof(newValue.Id), "AssetPair Id must be set");
            }
            
            if (newValue.BasePairId == null) 
                return;

            var baseAssetPair = await _assetPairsRepository.GetAsync(s => s.BaseAssetId == newValue.BasePairId);
            if (baseAssetPair.Any())
            {
                throw new InvalidOperationException($"BasePairId {newValue.BasePairId} does not exist");
            }

            var newBase =
                await _assetPairsRepository.GetAsync(s => s.Id != newValue.Id && s.BasePairId == newValue.BasePairId);
            if (newBase.Any())
            {
                throw new InvalidOperationException($"BasePairId {newValue.BasePairId} cannot be added twice");
            }    
        }

        private void ValidateId(string id, AssetPairContract contract)
        {
            if (contract?.Id != id)
            {
                throw new ArgumentException("Id must match with contract id");
            }
        }
    }
}