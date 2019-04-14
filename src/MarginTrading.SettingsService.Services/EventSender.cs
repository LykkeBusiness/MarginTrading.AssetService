using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using MarginTrading.SettingsService.Contracts.Enums;
using MarginTrading.SettingsService.Contracts.Messages;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using Microsoft.Extensions.Internal;

namespace MarginTrading.SettingsService.Services
{
    [UsedImplicitly]
    public class EventSender : IEventSender
    {
        private readonly IConvertService _convertService;
        private readonly ILog _log;
        private readonly ISystemClock _systemClock;

        private readonly IMessageProducer<SettingsChangedEvent> _settingsChangedMessageProducer;

        public EventSender(
            IRabbitMqService rabbitMqService,
            IConvertService convertService,
            ILog log,
            ISystemClock systemClock,
            string settingsChangedConnectionString,
            string settingsChangedExchangeName)
        {
            _convertService = convertService;
            _log = log;
            _systemClock = systemClock;

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
                _log.WriteError(nameof(EventSender), message.ToJson(), ex);
            }
        }
    }
}