// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Services
{
    public interface IEventSender
    {
        Task SendSettingsChangedEvent(string route, SettingsChangedSourceType sourceType, string changedEntityId = null);
    }
}