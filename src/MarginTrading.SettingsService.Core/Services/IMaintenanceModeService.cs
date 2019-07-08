// Copyright (c) 2019 Lykke Corp.

namespace MarginTrading.SettingsService.Core.Services
{
    public interface IMaintenanceModeService
    {
        bool CheckIsEnabled();

        void SetMode(bool isEnabled);
    }
}