// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MarginTrading.AssetService.Contracts.AssetPair;

namespace MarginTrading.AssetService.Core.Services
{
    public interface ICqrsMessageSender
    {
        Task SendAssetPairChangedEvent(AssetPairChangedEvent @event);
    }
}