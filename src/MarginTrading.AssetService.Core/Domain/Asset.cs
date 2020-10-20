// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
{
    public class Asset : IAsset
    {
        public Asset(string id, string name, int accuracy)
        {
            Id = id;
            Name = name;
            Accuracy = accuracy;
        }

        public string Id { get; }
        public string Name { get; }
        [Obsolete]
        public int Accuracy { get; }
    }
}