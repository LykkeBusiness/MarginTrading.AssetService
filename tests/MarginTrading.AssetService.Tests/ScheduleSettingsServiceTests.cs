using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Snow.Common.WorkingDays;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.Services;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Moq;
using Xunit;

namespace MarginTrading.AssetService.Tests
{
    public class ScheduleSettingsServiceTests
    {
        private readonly Mock<IBrokerSettingsApi> _brokerSettingsApiMock = new Mock<IBrokerSettingsApi>();
        private readonly Mock<IMarketSettingsRepository> _marketSettingsRepositoryMock = new Mock<IMarketSettingsRepository>();

        private const string PlatformId = "platform_id";
        private const string FakeMarketId = "fake_market_id";
        private const string BrokerId1 = "b1";
        private const string MarketId1 = "m1";
        private const string MarketId2 = "m2";
        
        [Theory]
        [InlineData(PlatformId)]
        [InlineData(MarketId1)]
        public async Task GetFiltered_WithMarketId_OnlyRequestedScheduleReturned(string marketId)
        {
            var sut = CreateSutInstance();

            ConfigureBrokerSettingsApi();
            ConfigureMarketSettingsRepository();

            var result = await sut.GetFilteredAsync(marketId);

            Assert.All(result, x => Assert.Equal(marketId, x.MarketId));
        }

        [Fact]
        public async Task GetFiltered_WithoutMarketId_AllSchedulesReturned()
        {
            var sut = CreateSutInstance();

            ConfigureBrokerSettingsApi();
            ConfigureMarketSettingsRepository();
            
            var result = await sut.GetFilteredAsync();
            
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public async Task GetFiltered_WithMultiSessions_IntervalsReturned()
        {
            var sut = CreateSutInstance();

            ConfigureBrokerSettingsApi();
            ConfigureMarketScheduleRepositoryWithMultiSessions();
            
            var result = await sut.GetFilteredAsync(FakeMarketId);
            
            Assert.Equal(3, result.Count);
            Assert.Single(result, x => x.Start.Time == TimeSpan.FromHours(10) && x.End.Time == TimeSpan.FromHours(8));
            Assert.Single(result, x => x.Start.Time == TimeSpan.FromHours(20) && x.End.Time == TimeSpan.FromHours(12));
        }

        [Fact]
        public async Task GetFiltered_WithMultiSessions_WithHalfWorkingDays_IntervalsReturned()
        {
            var sut = CreateSutInstance();

            ConfigureBrokerSettingsApi();
            ConfigureMarketScheduleRepositoryWithHalfWorkingDaysAndMultiSessions();
            
            var result = await sut.GetFilteredAsync(FakeMarketId);
            
            Assert.Equal(4, result.Count);
            Assert.Single(result, x => x.Start.Time == TimeSpan.FromHours(10) && x.End.Time == TimeSpan.FromHours(8));
            Assert.Single(result, x => x.Start.Time == TimeSpan.FromHours(20) && x.End.Time == TimeSpan.FromHours(12));
            Assert.Single(result, x =>
                x.Start.Time == TimeSpan.FromHours(14) && x.Start.Date == DateTime.Parse("2022-12-31") &&
                x.End.Time == TimeSpan.Zero && x.End.Date == DateTime.Parse("2022-12-31"));
        }

        private void ConfigureBrokerSettingsApi()
        {
            _brokerSettingsApiMock
                .Setup(x => x.GetScheduleInfoByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetBrokerSettingsScheduleResponse
                {
                    BrokerSettingsSchedule = new BrokerSettingsScheduleContract
                    {
                        BrokerId = BrokerId1, 
                        Weekends = new List<DayOfWeek>
                        {
                            DayOfWeek.Saturday
                        },
                        Holidays = new List<DateTime>(),
                        PlatformSchedule = new PlatformScheduleContract{HalfWorkingDays = new List<WorkingDay>()}
                    }
                });
        }

        private void ConfigureMarketSettingsRepository()
        {
            _marketSettingsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => new MarketSettings
                {
                    Id = id,
                    Holidays = new List<DateTime>(),
                    MarketSchedule = CreateMarketSchedule()
                });

            _marketSettingsRepositoryMock
                .Setup(x => x.GetAllMarketSettingsAsync())
                .ReturnsAsync(new[]
                {
                    new MarketSettings
                    {
                        Id = MarketId1,
                        Holidays = new List<DateTime>(),
                        MarketSchedule = CreateMarketSchedule()
                    },
                    new MarketSettings
                    {
                        Id = MarketId2,
                        Holidays = new List<DateTime>(),
                        MarketSchedule = CreateMarketSchedule()
                    }
                });
            
            MarketSchedule CreateMarketSchedule() => new MarketSchedule(
                new[] {TimeSpan.FromHours(8)},
                new[] {TimeSpan.FromHours(20)}, 
                "UTC", 
                Enumerable.Empty<string>());
        }

        private void ConfigureMarketScheduleRepositoryWithMultiSessions()
        {
            _marketSettingsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => new MarketSettings
                {
                    Id = id,
                    Holidays = new List<DateTime>(),
                    MarketSchedule = CreateMarketSchedule()
                });

            MarketSchedule CreateMarketSchedule() => new MarketSchedule(
                new[] {TimeSpan.FromHours(8), TimeSpan.FromHours(12)},
                new[] {TimeSpan.FromHours(10), TimeSpan.FromHours(20)},
                "UTC",
                Enumerable.Empty<string>());
        }

        private void ConfigureMarketScheduleRepositoryWithHalfWorkingDaysAndMultiSessions()
        {
            _marketSettingsRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => new MarketSettings
                {
                    Id = id,
                    Holidays = new List<DateTime>(),
                    MarketSchedule = CreateMarketSchedule()
                });
            
            MarketSchedule CreateMarketSchedule() => new MarketSchedule(
                new[] {TimeSpan.FromHours(8), TimeSpan.FromHours(12)},
                new[] {TimeSpan.FromHours(10), TimeSpan.FromHours(20)},
                "UTC",
                new List<string>{"2022-12-31 < 14:00:00"});
        }
        
        private IScheduleSettingsService CreateSutInstance(string brokerId = "brokerId")
        {
            return new ScheduleSettingsService(
                _brokerSettingsApiMock.Object,
                _marketSettingsRepositoryMock.Object,
                new PlatformSettings{PlatformMarketId = PlatformId},
                brokerId,
                EmptyLog.Instance);
        }
    }
}