using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Trading conditions management
    /// </summary>
    [Route("api/tradingConditions")]
    public class TradingConditionsController : Controller, ITradingConditionsApi
    {
        private readonly ITradingConditionsRepository _tradingConditionsRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public TradingConditionsController(
            ITradingConditionsRepository tradingConditionsRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _tradingConditionsRepository = tradingConditionsRepository;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of trading conditions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<List<TradingConditionContract>> List()
        {
            var data = await _tradingConditionsRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<ITradingCondition, TradingConditionContract>(x)).ToList();
        }

        /// <summary>
        /// Create new trading condition
        /// </summary>
        /// <param name="tradingCondition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<TradingConditionContract> Insert([FromBody] TradingConditionContract tradingCondition)
        {
            if (string.IsNullOrWhiteSpace(tradingCondition?.Id))
            {
                throw new ArgumentNullException(nameof(tradingCondition.Id), "asset Id must be set");
            }

            await _tradingConditionsRepository.InsertAsync(
                _convertService.Convert<TradingConditionContract, TradingCondition>(tradingCondition));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingCondition);

            return tradingCondition;
        }

        /// <summary>
        /// Get the trading condition
        /// </summary>
        /// <param name="tradingConditionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tradingConditionId}")]
        public async Task<TradingConditionContract> Get(string tradingConditionId)
        {
            var obj = await _tradingConditionsRepository.GetAsync(tradingConditionId);
            
            return _convertService.Convert<ITradingCondition, TradingConditionContract>(obj);
        }

        /// <summary>
        /// Get the default trading condition
        /// </summary>
        [HttpGet]
        [Route("{tradingConditionId}")]
        public async Task<TradingConditionContract> GetDefault()
        {
            var data = await _tradingConditionsRepository.GetAsync();

            // todo: search for a default condition instead of a first one
            return data.Select(x => _convertService.Convert<ITradingCondition, TradingConditionContract>(x)).First();
        }

        /// <summary>
        /// Update the trading condition
        /// </summary>
        /// <param name="tradingConditionId"></param>
        /// <param name="tradingCondition"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{tradingConditionId}")]
        public async Task<TradingConditionContract> Update(string tradingConditionId, 
            [FromBody] TradingConditionContract tradingCondition)
        {
            if (string.IsNullOrWhiteSpace(tradingCondition?.Id))
            {
                throw new ArgumentNullException(nameof(tradingCondition.Id), "asset Id must be set");
            }

            await _tradingConditionsRepository.ReplaceAsync(
                _convertService.Convert<TradingConditionContract, TradingCondition>(tradingCondition));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.TradingCondition);
            
            return tradingCondition;
        }
    }
}