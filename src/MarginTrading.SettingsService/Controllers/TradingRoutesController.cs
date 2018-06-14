using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.Routes;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Trading route management
    /// </summary>
    [Route("api/routes")]
    public class TradingRoutesController : Controller, ITradingRoutesApi
    {
        private readonly IAssetsRepository _assetsRepository;
        private readonly ITradingRoutesRepository _tradingRoutesRepository;
        private readonly ITradingConditionsRepository _tradingConditionsRepository;
        private readonly IAssetPairsRepository _assetPairsRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        private const string AnyValue = "*";
        
        public TradingRoutesController(
            IAssetsRepository assetsRepository,
            ITradingRoutesRepository tradingRoutesRepository,
            ITradingConditionsRepository tradingConditionsRepository,
            IAssetPairsRepository assetPairsRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _assetsRepository = assetsRepository;
            _tradingRoutesRepository = tradingRoutesRepository;
            _tradingConditionsRepository = tradingConditionsRepository;
            _assetPairsRepository = assetPairsRepository;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of trading routes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<List<MatchingEngineRouteContract>> List()
        {
            var data = await _tradingRoutesRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<ITradingRoute, MatchingEngineRouteContract>(x)).ToList();
        }

        /// <summary>
        /// Create new trading route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
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

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingRoute);

            return route;
        }

        /// <summary>
        /// Get the trading route
        /// </summary>
        /// <param name="routeId"></param>
        /// <returns></returns>
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
        /// <param name="routeId"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{routeId}")]
        public async Task<MatchingEngineRouteContract> Update(string routeId, 
            [FromBody] MatchingEngineRouteContract route)
        {
            await ValidateRoute(route);
            
            ValidateId(routeId, route);
            
            await _tradingRoutesRepository.UpdateAsync(
                _convertService.Convert<MatchingEngineRouteContract, TradingRoute>(route));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingRoute);
            
            return route;
        }

        /// <summary>
        /// Delete the trading route
        /// </summary>
        /// <param name="routeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{routeId}")]
        public async Task Delete(string routeId)
        {
            await _tradingRoutesRepository.DeleteAsync(routeId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingRoute);
        }

        private async Task ValidateRoute(MatchingEngineRouteContract route)
        {
            if (route == null)
            {
                throw new ArgumentNullException("route", "Model is incorrect");
            }
            
            if (string.IsNullOrWhiteSpace(route?.Id))
            {
                throw new ArgumentNullException(nameof(route.Id), "Route Id must be set");
            }

            if (!string.IsNullOrEmpty(route.TradingConditionId)
                && await _tradingConditionsRepository.GetAsync(route.TradingConditionId) == null)
            {
                throw new InvalidOperationException($"Trading condition {route.TradingConditionId} does not exist");
            }

            if (!string.IsNullOrEmpty(route.Instrument) 
                && await _assetPairsRepository.GetAsync(route.Instrument) == null)
            {
                throw new InvalidOperationException($"Asset pair {route.Instrument} does not exist");
            }

            if (string.IsNullOrEmpty(route.Asset))
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