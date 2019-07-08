// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MarginTrading.SettingsService.Contracts.AssetPair;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface ICqrsMessageSender
    {
        Task SendAssetPairChangedEvent(AssetPairChangedEvent @event);
    }
}