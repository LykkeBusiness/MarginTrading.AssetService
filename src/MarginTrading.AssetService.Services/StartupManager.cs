// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.Cqrs;
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
            _log.Info(nameof(StartupManager), "Trying to start underlyings cache.");
            _underlyingsCache.Start();
            _log.Info(nameof(StartupManager), "Started underlyings cache.");
            _log.Info(nameof(StartupManager), "Trying to start legacy assets cache.");
            _legacyAssetsCache.Start();
            _log.Info(nameof(StartupManager), "Started legacy assets cache.");
            _log.Info(nameof(StartupManager), "Trying to start startables.");
            StartStartables();
            _log.Info(nameof(StartupManager), "Started startables.");
            _log.Info(nameof(StartupManager), "Trying to start cqrs engine subscribers.");
            _cqrsEngine.StartSubscribers();
            _log.Info(nameof(StartupManager), "Started cqrs engine subscribers.");
            _log.Info(nameof(StartupManager), "Trying to start cqrs engine processes.");
            _cqrsEngine.StartProcesses();
            _log.Info(nameof(StartupManager), "Started cqrs engine subscribers.");
            _log.Info(nameof(StartupManager), "Trying to start cqrs engine publishers.");
            _cqrsEngine.StartPublishers();
            _log.Info(nameof(StartupManager), "Started cqrs engine subscribers.");

            await Task.CompletedTask;
        }

        private void StartStartables()
        {
            var counter = 1;
            foreach (var component in _starables)
            {
                var cName = component.GetType().Name;

                try
                {
                    _log.Info(nameof(StartupManager), $"Trying to start startable: #{counter}");
                    component.Start();
                    _log.Info(nameof(StartupManager), $"Started startable: #{counter}");
                    counter++;
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