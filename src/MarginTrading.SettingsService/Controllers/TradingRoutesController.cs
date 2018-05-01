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
        private readonly ITradingRoutesRepository _tradingRoutesRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public TradingRoutesController(
            ITradingRoutesRepository tradingRoutesRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _tradingRoutesRepository = tradingRoutesRepository;
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
            if (string.IsNullOrWhiteSpace(route?.Id))
            {
                throw new ArgumentNullException(nameof(route.Id), "route Id must be set");
            }

            await _tradingRoutesRepository.InsertAsync(
                _convertService.Convert<MatchingEngineRouteContract, TradingRoute>(route));

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
            if (string.IsNullOrWhiteSpace(route?.Id))
            {
                throw new ArgumentNullException(nameof(route.Id), "route Id must be set");
            }

            await _tradingRoutesRepository.ReplaceAsync(
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
    }
}