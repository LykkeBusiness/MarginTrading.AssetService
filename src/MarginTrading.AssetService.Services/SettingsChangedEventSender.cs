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
    public class SettingsChangedEventSender : ISettingsChangedEventSender
    {
        private readonly IConvertService _converter;
        private readonly ILogger<SettingsChangedEventSender> _logger;
        private readonly ISystemClock _systemClock;
        private readonly IMessageProducer<SettingsChangedEvent> _settingsChangedProducer;

        public SettingsChangedEventSender(
            IConvertService converter,
            ISystemClock systemClock,
            IMessageProducer<SettingsChangedEvent> settingsChangedProducer,
            ILogger<SettingsChangedEventSender> logger)
        {
            _converter = converter;
            _systemClock = systemClock;
            _logger = logger;
            _settingsChangedProducer = settingsChangedProducer;
        }

        public async Task Send(string route, SettingsChangedSourceType sourceType,
            string changedEntityId = null)
        {
            var message = new SettingsChangedEvent
            {
                Route = route,
                SettingsType = _converter.Convert<SettingsChangedSourceType, SettingsTypeContract>(sourceType),
                Timestamp = _systemClock.UtcNow.DateTime,
                ChangedEntityId = changedEntityId
            };

            try
            {
                await _settingsChangedProducer.ProduceAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event on settings changed, message: {MessageJson}", message.ToJson());
            }
        }
    }
}