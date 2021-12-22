// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Cqrs;
using Lykke.RabbitMqBroker;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Services;

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
        private readonly ILog _log;
        private readonly ICqrsEngine _cqrsEngine;
        private readonly IUnderlyingsCache _underlyingsCache;
        private readonly ILegacyAssetsCache _legacyAssetsCache;
        private readonly IEnumerable<IStartStop> _starables;

        public StartupManager(
            ILog log,
            ICqrsEngine cqrsEngine,
            IUnderlyingsCache underlyingsCache,
            ILegacyAssetsCache legacyAssetsCache,
            IEnumerable<IStartStop> starables)
        {
            _log = log;
            _cqrsEngine = cqrsEngine;
            _underlyingsCache = underlyingsCache;
            _legacyAssetsCache = legacyAssetsCache;
            _starables = starables;
        }

        public async Task StartAsync()
        {
            _underlyingsCache.Start();
            _legacyAssetsCache.Start();
            _cqrsEngine.StartSubscribers();
            _cqrsEngine.StartProcesses();
            _cqrsEngine.StartPublishers();
            StartStartables();

            await Task.CompletedTask;
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
                    _log.WriteError(nameof(StartupManager), $"Couldn't start component {cName}.",e) ;
                    throw;
                }
            }
        }
    }
}