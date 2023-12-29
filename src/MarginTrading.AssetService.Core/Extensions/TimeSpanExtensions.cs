// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace MarginTrading.AssetService.Core.Extensions
{
    internal static class TimeSpanExtensions
    {
        public static TimeSpan[] GetValueOrDefault(this TimeSpan[] value, TimeSpan defaultValue) =>
            value.Any() ? value : new[] { defaultValue };
    }
}