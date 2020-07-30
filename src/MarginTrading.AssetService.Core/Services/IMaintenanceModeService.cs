﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace MarginTrading.AssetService.Core.Services
{
    public interface IMaintenanceModeService
    {
        bool CheckIsEnabled();

        void SetMode(bool isEnabled);
    }
}