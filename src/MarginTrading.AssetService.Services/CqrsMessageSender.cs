// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;

namespace MarginTrading.AssetService.Services
{
    [UsedImplicitly]
    public class CqrsMessageSender : ICqrsMessageSender
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly CqrsContextNamesSettings _contextNames;
        private readonly IConvertService _convertService;
        private readonly ILog _log;

        public CqrsMessageSender(
            ICqrsEngine cqrsEngine,
            CqrsContextNamesSettings contextNames,
            IConvertService convertService,
            ILog log)
        {
            _cqrsEngine = cqrsEngine;
            _contextNames = contextNames;
            _convertService = convertService;
            _log = log;
        }

        public async Task SendEvent<TEvent>(TEvent @event)
        {
            try
            {
                _cqrsEngine.PublishEvent(@event, _contextNames.AssetService);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(CqrsMessageSender), nameof(TEvent), ex);
            }
        }

        public Task SendEntityCreatedEvent<TModel, TContract, TEvent>(TModel newValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
            => SendEntityChangedEvent<TModel, TContract, TEvent>(null, newValue,
                username, correlationId, ChangeType.Creation);

        public Task SendEntityEditedEvent<TModel, TContract, TEvent>(TModel oldValue,
            TModel newValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
            => SendEntityChangedEvent<TModel, TContract, TEvent>(oldValue, newValue,
                username, correlationId, ChangeType.Edition);

        public Task SendEntityDeletedEvent<TModel, TContract, TEvent>(TModel oldValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
            => SendEntityChangedEvent<TModel, TContract, TEvent>(oldValue, null,
                username, correlationId, ChangeType.Deletion);

        private async Task SendEntityChangedEvent<TModel, TContract, TEvent>(TModel oldValue, TModel newValue,
            string username, string correlationId, ChangeType changeType)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
        {
            await SendEvent(new TEvent()
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = correlationId,
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = _convertService.Convert<TModel, TContract>(oldValue),
                NewValue = _convertService.Convert<TModel, TContract>(newValue),
            });
        }
    }
}