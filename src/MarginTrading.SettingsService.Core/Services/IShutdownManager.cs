// Copyright (c) 2019 Lykke Corp.

using System.Threading.Tasks;

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
