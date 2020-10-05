// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ICqrsMessageSender
    {
        Task SendEvent<TEvent>(TEvent @event);
    }
}