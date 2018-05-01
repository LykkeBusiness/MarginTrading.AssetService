using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.SettingsService.Controllers
{
    /// <summary>
    /// MT Core service maintenance management
    /// </summary>
    [Route("api/service/maintenance")]
    public class ServiceMaintenanceController : Controller, IServiceMaintenanceApi
    {
        private readonly IMaintenanceModeService _maintenanceModeService;
        private readonly IEventSender _eventSender;

        public ServiceMaintenanceController(
            IMaintenanceModeService maintenanceModeService,
            IEventSender eventSender)
        {
            _maintenanceModeService = maintenanceModeService;
            _eventSender = eventSender;
        }
        
        /// <summary>
        /// Get current service state
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public Task<bool> Get()
        {
            return Task.FromResult(_maintenanceModeService.CheckIsEnabled());
        }

        /// <summary>
        /// Switch maintenance mode
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public Task<bool> Post([FromBody] bool enabled)
        {
            _maintenanceModeService.SetMode(enabled);
            
            _eventSender.SendSettingsChangedEvent($"{Request.Path}", SettingsChangedSourceType.ServiceMaintenance);
            
            return Task.FromResult(true);
        }
    }
}