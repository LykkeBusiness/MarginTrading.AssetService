using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Core.Settings;
using Microsoft.Extensions.Internal;

namespace MarginTrading.SettingsService.Services
{
    [UsedImplicitly]
    public class CqrsMessageSender : ICqrsMessageSender
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly CqrsContextNamesSettings _contextNames;

        public CqrsMessageSender(
            ISystemClock systemClock,
            ICqrsEngine cqrsEngine,
            CqrsContextNamesSettings contextNames)
        {
            _cqrsEngine = cqrsEngine;
            _contextNames = contextNames;
        }

        public Task SendAssetPairChangedEvent(AssetPairChangedEvent @event)
        {
            _cqrsEngine.PublishEvent(@event, _contextNames.SettingsService);
            
            return Task.CompletedTask;
        }
    }
}