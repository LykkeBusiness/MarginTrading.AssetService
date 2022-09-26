using System;
using System.Threading.Tasks;
using Lykke.Snow.Common.Correlation;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Core.Services;

namespace MarginTrading.AssetService.Services
{
    public class CqrsEntityChangedSender : ICqrsEntityChangedSender
    {
        private readonly ICqrsMessageSender _messageSender;
        private readonly IConvertService _convertService;
        private readonly CorrelationContextAccessor _correlationContextAccessor;

        public CqrsEntityChangedSender(ICqrsMessageSender messageSender, IConvertService convertService, CorrelationContextAccessor correlationContextAccessor)
        {
            _messageSender = messageSender;
            _convertService = convertService;
            _correlationContextAccessor = correlationContextAccessor;
        }
        
        public Task SendEntityCreatedEvent<TModel, TContract, TEvent>(TModel newValue, string username)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
            => SendEntityChangedEvent<TModel, TContract, TEvent>(null, newValue, username, ChangeType.Creation);

        public Task SendEntityEditedEvent<TModel, TContract, TEvent>(TModel oldValue, TModel newValue, string username)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
            => SendEntityChangedEvent<TModel, TContract, TEvent>(oldValue, newValue, username, ChangeType.Edition);

        public Task SendEntityDeletedEvent<TModel, TContract, TEvent>(TModel oldValue, string username)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
            => SendEntityChangedEvent<TModel, TContract, TEvent>(oldValue, null, username, ChangeType.Deletion);

        private async Task SendEntityChangedEvent<TModel, TContract, TEvent>(TModel oldValue, TModel newValue, string username, ChangeType changeType)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class
        {
            await _messageSender.SendEvent(new TEvent
            {
                Username = username,
                ChangeType = changeType,
                CorrelationId = _correlationContextAccessor.GetOrGenerateCorrelationId(),
                EventId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = _convertService.Convert<TModel, TContract>(oldValue),
                NewValue = _convertService.Convert<TModel, TContract>(newValue)
            });
        }
    }
}