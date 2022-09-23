// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    [UsedImplicitly]
    public class CqrsMessageSender : ICqrsMessageSender
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly CqrsContextNamesSettings _contextNames;
        private readonly ILogger<CqrsMessageSender> _logger;

        public CqrsMessageSender(
            ICqrsEngine cqrsEngine,
            CqrsContextNamesSettings contextNames,
            IConvertService convertService,
            ILogger<CqrsMessageSender> logger)
        {
            _cqrsEngine = cqrsEngine;
            _contextNames = contextNames;
            _logger = logger;
        }

        public Task SendEvent<TEvent>(TEvent @event)
        {
            try
            {
                _cqrsEngine.PublishEvent(@event, _contextNames.AssetService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventName}", typeof(TEvent).Name);
            }
            
            return Task.CompletedTask;
        }
    }
}