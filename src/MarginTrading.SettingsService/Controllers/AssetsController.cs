using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Client;
using MarginTrading.SettingsService.Client.Asset;
using MarginTrading.SettingsService.Client.AssetPair;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Assets management
    /// </summary>
    [Route("api/assets")]
    public class AssetsController : Controller, IAssetsApi
    {
        private readonly IAssetsRepository _assetsRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public AssetsController(
            IAssetsRepository assetsRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _assetsRepository = assetsRepository;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of assets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<List<AssetContract>> List()
        {
            var data = await _assetsRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<Asset, AssetContract>(x)).ToList();
        }

        /// <summary>
        /// Create new asset
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<AssetContract> Insert([FromBody] AssetContract asset)
        {
            if (string.IsNullOrWhiteSpace(asset?.Id))
            {
                throw new ArgumentNullException(nameof(asset.Id), "asset Id must be set");
            }
            
            await _assetsRepository.InsertAsync(_convertService.Convert<AssetContract, Asset>(asset));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.Asset);

            return asset;
        }

        /// <summary>
        /// Get the asset
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{assetId}")]
        public async Task<AssetContract> Get(string assetId)
        {
            var obj = await _assetsRepository.GetAsync(assetId);
            
            return _convertService.Convert<Asset, AssetContract>(obj);
        }

        /// <summary>
        /// Update the asset
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{assetId}")]
        public async Task<AssetContract> Update(string assetId, [FromBody] AssetContract asset)
        {
            if (string.IsNullOrWhiteSpace(asset?.Id))
            {
                throw new ArgumentNullException(nameof(asset.Id), "asset Id must be set");
            }

            await _assetsRepository.ReplaceAsync(_convertService.Convert<AssetContract, Asset>(asset));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.Asset);
            
            return asset;
        }

        /// <summary>
        /// Delete the asset
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{assetId}")]
        public async Task Delete(string assetId)
        {
            await _assetsRepository.DeleteAsync(assetId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.Asset);
        }
    }
}