// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.AssetPair;
using MarginTrading.AssetService.Contracts.Common;
using MarginTrading.AssetService.Contracts.Enums;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ICqrsMessageSender
    {
        Task SendEvent<TEvent>(TEvent @event);

        Task SendEntityCreatedEvent<TModel, TContract, TEvent>(TModel newValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class;

        Task SendEntityEditedEvent<TModel, TContract, TEvent>(TModel oldValue,
            TModel newValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class;

        Task SendEntityDeletedEvent<TModel, TContract, TEvent>(TModel oldValue,
            string username, string correlationId)
            where TEvent : EntityChangedEvent<TContract>, new()
            where TModel : class;
    }
}