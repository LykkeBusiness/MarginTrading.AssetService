// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Routes;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Extensions;
using MarginTrading.AssetService.Middleware;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// Trading route management
    /// </summary>
    [Authorize]
    [Route("api/routes")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class TradingRoutesController : Controller, ITradingRoutesApi
    {
        private readonly IAssetsRepository _assetsRepository;
        private readonly ITradingRoutesRepository _tradingRoutesRepository;
        private readonly ITradingConditionsService _tradingConditionsService;
        private readonly IAssetPairService _assetPairsService;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        private const string AnyValue = "*";
        
        public TradingRoutesController(
            IAssetsRepository assetsRepository,
            ITradingRoutesRepository tradingRoutesRepository,
            ITradingConditionsService tradingConditionsService,
            IAssetPairService assetPairsService,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _assetsRepository = assetsRepository;
            _tradingRoutesRepository = tradingRoutesRepository;
            _tradingConditionsService = tradingConditionsService;
            _assetPairsService = assetPairsService;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of trading routes
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<List<MatchingEngineRouteContract>> List()
        {
            var data = await _tradingRoutesRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<ITradingRoute, MatchingEngineRouteContract>(x)).ToList();
        }

        /// <summary>
        /// Get the list of trading routes, with optional pagination
        /// </summary>
        [HttpGet]
        [Route("by-pages")]
        public async Task<PaginatedResponseContract<MatchingEngineRouteContract>> ListByPages(int? skip = null, int? take = null)
        {
            ApiValidationHelper.ValidatePagingParams(skip, take);
            
            var data = await _tradingRoutesRepository.GetByPagesAsync(skip, take);
            
            return new PaginatedResponseContract<MatchingEngineRouteContract>(
                contents: data.Contents.Select(x => _convertService.Convert<ITradingRoute, MatchingEngineRouteContract>(x)).ToList(),
                start: data.Start,
                size: data.Size,
                totalSize: data.TotalSize
            );
        }

        /// <summary>
        /// Create new trading route
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<MatchingEngineRouteContract> Insert([FromBody] MatchingEngineRouteContract route)
        {
            await ValidateRoute(route);

            if (!await _tradingRoutesRepository.TryInsertAsync(
                _convertService.Convert<MatchingEngineRouteContract, TradingRoute>(route)))
            {
                throw new ArgumentException($"Trading route with id {route.Id} already exists", nameof(route.Id));
            }

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.TradingRoute, route.Id);

            return route;
        }

        /// <summary>
        /// Get the trading route
        /// </summary>
        [HttpGet]
        [Route("{routeId}")]
        public async Task<MatchingEngineRouteContract> Get(string routeId)
        {
            var obj = await _tradingRoutesRepository.GetAsync(routeId);
            
            return _convertService.Convert<ITradingRoute, MatchingEngineRouteContract>(obj);
        }

        /// <summary>
        /// Update the trading route
        /// </summary>
        [HttpPut]
        [Route("{routeId}")]
        public async Task<MatchingEngineRouteContract> Update(string routeId, 
            [FromBody] MatchingEngineRouteContract route)
        {
            await ValidateRoute(route);
            
            ValidateId(routeId, route);
            
            await _tradingRoutesRepository.UpdateAsync(
                _convertService.Convert<MatchingEngineRouteContract, TradingRoute>(route));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.TradingRoute, routeId);
            
            return route;
        }

        /// <summary>
        /// Delete the trading route
        /// </summary>
        [HttpDelete]
        [Route("{routeId}")]
        public async Task Delete(string routeId)
        {
            await _tradingRoutesRepository.DeleteAsync(routeId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", 
                SettingsChangedSourceType.TradingRoute, routeId);
        }

        private async Task ValidateRoute(MatchingEngineRouteContract route)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route), "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(route?.Id))
            {
                throw new ArgumentNullException(nameof(route.Id), "Route Id must be set");
            }

            if (route.Type != null && !Enum.IsDefined(typeof(OrderDirectionContract), route.Type))
            {
                throw new ArgumentNullException(nameof(route.Type), "Route Type is set to an incorrect value");
            }

            if (!string.IsNullOrEmpty(route.TradingConditionId)
                && await _tradingConditionsService.GetAsync(route.TradingConditionId) == null)
            {
                throw new InvalidOperationException($"Trading condition {route.TradingConditionId} does not exist");
            }

            if (!string.IsNullOrEmpty(route.Instrument) 
                && await _assetPairsService.GetByIdAsync(route.Instrument) == null)
            {
                throw new InvalidOperationException($"Asset pair {route.Instrument} does not exist");
            }

            if (string.IsNullOrEmpty(route.Asset) || route.Asset == AnyValue)
            {
                route.Asset = AnyValue;
            }
            else if (await _assetsRepository.GetAsync(route.Asset) == null)
            {
                throw new InvalidOperationException($"Asset {route.Asset} does not exist");
            }
        }

        private void ValidateId(string id, MatchingEngineRouteContract contract)
        {
            if (contract?.Id != id)
            {
                throw new ArgumentException("Id must match with contract id");
            }
        }
    }
}