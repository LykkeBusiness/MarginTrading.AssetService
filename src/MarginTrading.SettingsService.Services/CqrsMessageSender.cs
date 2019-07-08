// Copyright (c) 2019 Lykke Corp.

using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;

namespace MarginTrading.SettingsService.Services
{
    [UsedImplicitly]
    public class CqrsMessageSender : ICqrsMessageSender
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly CqrsContextNamesSettings _contextNames;
        private readonly ILog _log;

        public CqrsMessageSender(
            ICqrsEngine cqrsEngine,
            CqrsContextNamesSettings contextNames,
            ILog log)
        {
            _cqrsEngine = cqrsEngine;
            _contextNames = contextNames;
            _log = log;
        }

        public async Task SendAssetPairChangedEvent(AssetPairChangedEvent @event)
        {
            try
            {
                _cqrsEngine.PublishEvent(@event, _contextNames.SettingsService);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CqrsMessageSender), nameof(SendAssetPairChangedEvent), ex);
            }
        }
    }
}