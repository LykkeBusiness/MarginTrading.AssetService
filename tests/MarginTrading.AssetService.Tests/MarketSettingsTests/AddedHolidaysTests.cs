// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Extensions;

using Xunit;

namespace MarginTrading.AssetService.Tests.MarketSettingsTests
{
    /// <summary>
    /// Dates generation implies that time portion of the date is random.
    /// This perfectly makes sense because we don't care about time portion
    /// in algorithms tested so it shouldn't affect the results.
    /// On the contrary, if it ever affects the results, it means that
    /// algorithms are not implemented correctly.
    /// </summary>
    public class AddedHolidaysTests
    {
        [Fact]
        public void ShouldReturnHolidays_WhenNewHolidaysAdded()
        {
            var current = WithHolidaysOnly(Jan(1), Jan(2), Jan(3));
            var future = WithHolidaysOnly(Jan(1), Jan(2), Jan(3), Jan(4), Jan(5));

            var addedHolidays = current.AddedHolidays(future).ToList();

            addedHolidays.AssertLength(2);
            addedHolidays.AssertHasDays(Jan(4), Jan(5));
        }

        [Fact]
        public void ShouldReturnHolidays_WhenMixingCurrentAndFutureHolidays()
        {
            var current = WithHolidaysOnly(Jan(1), Jan(3), Jan(5));
            var future = WithHolidaysOnly(Jan(2), Jan(4), Jan(5));

            var addedHolidays = current.AddedHolidays(future).ToList();

            addedHolidays.AssertLength(2);
            addedHolidays.AssertHasDays(Jan(2), Jan(4));
        }

        [Fact]
        public void ShouldReturnNothing_WhenNoHolidaysAdded()
        {
            var current = WithHolidaysOnly(Jan(1), Jan(2), Jan(3));
            var future = WithHolidaysOnly(Jan(1), Jan(2), Jan(3));

            var addedHolidays = current.AddedHolidays(future).ToList();

            Assert.Empty(addedHolidays);
        }

        [Fact]
        public void ShouldReturnNothing_WhenNoHolidaysInFutureSettings()
        {
            var current = WithHolidaysOnly(Jan(1), Jan(2), Jan(3));
            var future = WithHolidaysOnly();

            var addedHolidays = current.AddedHolidays(future).ToList();

            Assert.Empty(addedHolidays);
        }

        [Fact]
        public void ShouldReturnAll_WhenNoHolidaysInCurrentSettings()
        {
            var current = WithHolidaysOnly();
            var future = WithHolidaysOnly(Jan(1), Jan(2), Jan(3));

            var addedHolidays = current.AddedHolidays(future).ToList();

            addedHolidays.AssertLength(3);
            addedHolidays.AssertHasDays(Jan(1), Jan(2), Jan(3));
        }

        [Fact]
        public void ShouldReturnHolidays_WhenNewHolidaysAddedInDifferentYears()
        {
            var current = WithHolidaysOnly(Feb(1, 2020), Feb(2, 2020), Feb(3, 2020));
            var future = WithHolidaysOnly(Feb(1, 2021), Feb(2, 2021), Feb(3, 2021));

            var addedHolidays = current.AddedHolidays(future).ToList();

            addedHolidays.AssertLength(3);
            addedHolidays.AssertHasDays(
                Feb(1, 2021),
                Feb(2, 2021),
                Feb(3, 2021));
        }

        private static DateTime Jan(int day, int year = 2000) => DateWithRandomTime(1, day, year);

        private static DateTime Feb(int day, int year = 2000) => DateWithRandomTime(2, day, year);

        private static DateTime DateWithRandomTime(int month, int day, int year)
        {
            Random rnd = new Random();
            return new DateTime(year, month, day,
                hour: rnd.Next(0, 23),
                minute: rnd.Next(0, 59),
                second: rnd.Next(0, 59),
                DateTimeKind.Utc);
        }

        private static MarketSettings WithHolidaysOnly(params DateTime[] holidays) =>
            new MarketSettings { Holidays = new List<DateTime>(holidays) };
    }

    internal static class DateTimeListAssertExtensions
    {
        private static void AssertHasDay(this IEnumerable<DateTime> source, DateTime day)
        {
            Assert.Single(source, d => d.Date.Equals(day.Date));
        }

        internal static void AssertHasDays(this IEnumerable<DateTime> source, params DateTime[] days)
        {
            Assert.All(days, source.AssertHasDay);
        }

        internal static void AssertLength(this IEnumerable<DateTime> source, int length)
        {
            Assert.Equal(length, source.Count());
        }
    }
}