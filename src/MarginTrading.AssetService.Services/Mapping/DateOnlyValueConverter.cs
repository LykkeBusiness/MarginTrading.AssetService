// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using AutoMapper;

namespace MarginTrading.AssetService.Services.Mapping
{
    internal sealed class DateOnlyValueConverter : IValueConverter<DateTime?, DateOnly?>
    {
        public DateOnly? Convert(DateTime? sourceMember, ResolutionContext context)
        {
            return sourceMember.HasValue ? DateOnly.FromDateTime(sourceMember.Value) : (DateOnly?)null;
        }
    }
}