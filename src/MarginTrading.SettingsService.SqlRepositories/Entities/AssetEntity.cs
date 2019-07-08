﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.SettingsService.Core.Interfaces;

namespace MarginTrading.SettingsService.SqlRepositories.Entities
{
    public class AssetEntity : IAsset
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Accuracy { get; set; }
    }
}