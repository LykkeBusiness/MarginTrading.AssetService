// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface IAsset
    {
        string Id { get; }
        string Name { get; }
        int Accuracy { get; }
    }
}
