// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Lykke.Cqrs;
using Lykke.RabbitMqBroker;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly ILogger<StartupManager> _logger;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ILegacyAssetsCache _legacyAssetsCache;
        private readonly IEnumerable<IStartStop> _starables;
        private readonly ICqrsEngine _cqrsEngine;

        public StartupManager(
            IUnderlyingsCache underlyingsCache,
            ILegacyAssetsCache legacyAssetsCache,
            IEnumerable<IStartStop> starables,
            ILogger<StartupManager> logger,
            ICqrsEngine cqrsEngine)
        {
            _underlyingsCache = underlyingsCache;
            _legacyAssetsCache = legacyAssetsCache;
            _starables = starables;
            _logger = logger;
            _cqrsEngine = cqrsEngine;
        }

        public void Start()
        {
            _underlyingsCache.Start();
            _legacyAssetsCache.Start();
            _cqrsEngine.StartAll();
            StartStartables();
        }

        private void StartStartables()
        {
            foreach (var component in _starables)
            {
                var cName = component.GetType().Name;

                try
                {
                    component.Start();
                }
                catch (Exception e)
                {
                    _logger.LogError(e,  "Couldn't start component {ComponentName}", cName);
                    throw;
                }
            }
        }
    }
}