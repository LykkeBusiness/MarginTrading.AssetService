using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Client;
using MarginTrading.SettingsService.Client.Market;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Interfaces;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// Markets management
    /// </summary>
    [Route("api/markets")]
    public class MarketsController : Controller, IMarketsApi
    {
        private readonly IMarketRepository _marketRepository;
        private readonly IConvertService _convertService;
        private readonly IEventSender _eventSender;
        
        public MarketsController(
            IMarketRepository marketRepository,
            IConvertService convertService,
            IEventSender eventSender)
        {
            _marketRepository = marketRepository;
            _convertService = convertService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get the list of Markets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<List<MarketContract>> List()
        {
            var data = await _marketRepository.GetAsync();
            
            return data.Select(x => _convertService.Convert<IMarket, MarketContract>(x)).ToList();
        }
        
        /// <summary>
        /// Create new market
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<MarketContract> Insert([FromBody] MarketContract market)
        {
            if (string.IsNullOrWhiteSpace(market?.Id))
            {
                throw new ArgumentNullException(nameof(market.Id), "market Id must be set");
            }
            
            await _marketRepository.InsertAsync(_convertService.Convert<MarketContract, Market>(market));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.Market);

            return market;
        }

        /// <summary>
        /// Get the market
        /// </summary>
        /// <param name="marketId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{marketId}")]
        public async Task<MarketContract> Get(string marketId)
        {
            var obj = await _marketRepository.GetAsync(marketId);
            
            return _convertService.Convert<IMarket, MarketContract>(obj);
        }

        /// <summary>
        /// Update the market
        /// </summary>
        /// <param name="marketId"></param>
        /// <param name="market"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{marketId}")]
        public async Task<MarketContract> Update(string marketId, [FromBody] MarketContract market)
        {
            if (string.IsNullOrWhiteSpace(market?.Id))
            {
                throw new ArgumentNullException(nameof(market.Id), "market Id must be set");
            }

            await _marketRepository.ReplaceAsync(_convertService.Convert<MarketContract, Market>(market));

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.Market);
            
            return market;
        }

        /// <summary>
        /// Delete the market
        /// </summary>
        /// <param name="marketId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{marketId}")]
        public async Task Delete(string marketId)
        {
            await _marketRepository.DeleteAsync(marketId);

            await _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.Market);
        }
    }
}