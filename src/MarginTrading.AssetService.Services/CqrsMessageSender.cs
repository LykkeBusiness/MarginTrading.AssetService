// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;

namespace MarginTrading.AssetService.Services
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
                _cqrsEngine.PublishEvent(@event, _contextNames.AssetService);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CqrsMessageSender), nameof(SendAssetPairChangedEvent), ex);
            }
        }
    }
}