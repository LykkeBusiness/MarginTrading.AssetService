using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.Asset;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
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
            
            return data.Select(x => _convertService.Convert<IAsset, AssetContract>(x)).ToList();
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

            if (!await _assetsRepository.TryInsertAsync(_convertService.Convert<AssetContract, Asset>(asset)))
            {
                throw new ArgumentException($"Asset with id {asset.Id} already exists", nameof(asset.Id));
            }

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
            
            return _convertService.Convert<IAsset, AssetContract>(obj);
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
            ValidateId(assetId, asset);
            
            if (string.IsNullOrWhiteSpace(asset?.Id))
            {
                throw new ArgumentNullException(nameof(asset.Id), "asset Id must be set");
            }

            await _assetsRepository.UpdateAsync(_convertService.Convert<AssetContract, Asset>(asset));

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

        private void ValidateId(string id, AssetContract contract)
        {
            if (contract?.Id != id)
            {
                throw new ArgumentException("Id must match with contract id");
            }
        }
    }
}