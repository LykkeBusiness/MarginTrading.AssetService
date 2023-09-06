// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarginTrading.AssetService.Controllers
{
    /// <summary>
    /// MT Core service maintenance management
    /// </summary>
    [Authorize]
    [Route("api/service/maintenance")]
    [MiddlewareFilter(typeof(RequestLoggingPipeline))]
    public class ServiceMaintenanceController : Controller, IServiceMaintenanceApi
    {
        private readonly IMaintenanceModeService _maintenanceModeService;
        private readonly ISettingsChangedEventSender _settingsChangedEventSender;

        public ServiceMaintenanceController(
            IMaintenanceModeService maintenanceModeService,
            ISettingsChangedEventSender settingsChangedEventSender)
        {
            _maintenanceModeService = maintenanceModeService;
            _settingsChangedEventSender = settingsChangedEventSender;
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
            
            _settingsChangedEventSender.Send($"{Request.Path}", SettingsChangedSourceType.ServiceMaintenance);
            
            return Task.FromResult(true);
        }
    }
}