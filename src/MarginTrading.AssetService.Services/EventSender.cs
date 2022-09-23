// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.Messages;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Services
{
    [UsedImplicitly]
    public class EventSender : IEventSender
    {
        private readonly IConvertService _convertService;
        private readonly ILogger<EventSender> _logger;
        private readonly ISystemClock _systemClock;

        private readonly Lykke.RabbitMqBroker.Publisher.IMessageProducer<SettingsChangedEvent> _settingsChangedMessageProducer;

        public EventSender(
            IRabbitMqService rabbitMqService,
            IConvertService convertService,
            ISystemClock systemClock,
            string settingsChangedConnectionString,
            string settingsChangedExchangeName,
            ILogger<EventSender> logger)
        {
            _convertService = convertService;
            _systemClock = systemClock;
            _logger = logger;

            _settingsChangedMessageProducer =
                rabbitMqService.GetProducer(settingsChangedConnectionString, settingsChangedExchangeName, true,
                    rabbitMqService.GetJsonSerializer<SettingsChangedEvent>());
        }
        
        public async Task SendSettingsChangedEvent(string route, SettingsChangedSourceType sourceType,
            string changedEntityId = null)
        {
            var message = new SettingsChangedEvent
            {
                Route = route,
                SettingsType = _convertService.Convert<SettingsChangedSourceType, SettingsTypeContract>(sourceType),
                Timestamp = _systemClock.UtcNow.DateTime,
                ChangedEntityId = changedEntityId,
            };

            try
            {
                await _settingsChangedMessageProducer.ProduceAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event on settings changed, message: {MessageJson}", message.ToJson());
            }
        }
    }
}