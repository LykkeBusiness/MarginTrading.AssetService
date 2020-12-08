using System;

namespace MarginTrading.AssetService.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime TrimMilliseconds(this DateTime src)
        {
            if (src == DateTime.MinValue)
                return src;

            return src.AddTicks(-(src.Ticks % TimeSpan.TicksPerSecond));
        }
    }
}