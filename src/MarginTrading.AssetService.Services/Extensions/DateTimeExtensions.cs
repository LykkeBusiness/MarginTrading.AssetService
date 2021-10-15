using System;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns start of the day (apply time interval 00:00:00) for a given DateTime? value
        /// </summary>
        public static DateTime? StartOfDay(this DateTime? dateTime)
        {
            return dateTime?.StartOfDay();
        }

        /// <summary>
        /// Returns start of the day (apply time interval 00:00:00) for a given DateTime value
        /// </summary>
        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }
    }
}