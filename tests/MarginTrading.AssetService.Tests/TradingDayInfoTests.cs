using System;

using Lykke.Snow.Common.TradingDays;

using MarginTrading.AssetService.Core.Domain;

using Xunit;

namespace MarginTrading.AssetService.Tests
{
    public class TradingDayInfoTests
    {
        [Fact]
        public void Constructor_ValidParameters_CreatesTradingDayInfo()
        {
            // Arrange
            DateTime timestamp = DateTime.Now;
            bool isTradingEnabled = true;
            TradingDay lastTradingDay = new TradingDay(timestamp);
            bool isBusinessDay = true;
            DateTime nextTradingDayStart = timestamp.AddDays(1);

            // Act
            TradingDayInfo tradingDayInfo = new TradingDayInfo(
                timestamp,
                isTradingEnabled,
                lastTradingDay,
                isBusinessDay,
                nextTradingDayStart);

            // Assert
            Assert.Equal(isTradingEnabled, tradingDayInfo.IsTradingEnabled);
            Assert.Equal(lastTradingDay, tradingDayInfo.LastTradingDay);
            Assert.Equal(isBusinessDay, tradingDayInfo.IsBusinessDay);
            Assert.Equal(nextTradingDayStart, tradingDayInfo.NextTradingDayStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Constructor_InvalidLastTradingDay_ThrowsArgumentOutOfRangeException(bool isTradingEnabled)
        {
            // Arrange
            DateTime timestamp = DateTime.Now;
            TradingDay lastTradingDay = new TradingDay(timestamp.AddDays(1));
            bool isBusinessDay = true;
            DateTime nextTradingDayStart = timestamp.AddDays(2);

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TradingDayInfo(
                timestamp,
                isTradingEnabled,
                lastTradingDay,
                isBusinessDay,
                nextTradingDayStart));
        }

        [Fact]
        public void Constructor_InvalidNextTradingDayStartLessThanOrEqualToTimestamp_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            DateTime timestamp = DateTime.Now;
            bool isTradingEnabled = true;
            TradingDay lastTradingDay = new TradingDay(timestamp);
            bool isBusinessDay = true;
            DateTime nextTradingDayStart = timestamp;

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TradingDayInfo(
                timestamp,
                isTradingEnabled,
                lastTradingDay,
                isBusinessDay,
                nextTradingDayStart));
        }

        [Fact]
        public void Constructor_InvalidNextTradingDayStartLessThanLastTradingDay_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            DateTime timestamp = DateTime.Now;
            bool isTradingEnabled = false;
            TradingDay lastTradingDay = new TradingDay(timestamp);
            bool isBusinessDay = true;
            DateTime nextTradingDayStart = timestamp.AddDays(-1);
        
            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new TradingDayInfo(
                timestamp,
                isTradingEnabled,
                lastTradingDay,
                isBusinessDay,
                nextTradingDayStart));
        }
    }
}