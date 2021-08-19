// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Core.Domain;
using MarginTrading.AssetService.Core.Interfaces;
using MarginTrading.AssetService.Core.Services;
using MarginTrading.AssetService.Core.Settings;
using MarginTrading.AssetService.Services;
using Microsoft.Extensions.Internal;
using Moq;
using Xunit;

namespace MarginTrading.AssetService.Tests
{
    public class MarketDayOffServiceTests
    {
        [Theory]
        [InlineData("Market1.1", false, "2019-10-11", "2019-10-11 23:00:00")]
        [InlineData("Market1.2", true, "2019-10-11", "2019-10-12 02:00:00")]
        [InlineData("Market1.3", true, "2019-10-11", "2019-10-12 00:00:00")]
        [InlineData("Market2.1", true, "2019-10-11", "2019-10-14 00:00:00")]
        [InlineData("Market2.2", false, "2019-10-11", "2019-10-12 01:00:00")]
        [InlineData("Market2.3", true, "2019-10-11", "2019-10-14 00:00:00")]
        [InlineData("Market2.4", false, "2019-10-10", "2019-10-11 22:00:00")]
        [InlineData("Market2.5", true, "2019-10-11", "2019-10-12 00:00:00")]
        [InlineData("Market2.6", true, "2019-10-11", "2019-10-12 00:00:00")]
        [InlineData("Market3.1", false, "2019-10-10", "2019-10-12 00:00:00")]
        [InlineData("Market3.2", false, "2019-10-10", "2019-10-13 00:00:00")]
        [InlineData("Market3.3", false, "2019-10-10", "2019-10-15 00:00:00")]
        [InlineData("Market4.1", true, "2019-10-11", "2019-10-15 00:00:00")]
        [InlineData("Market4.2", true, "2019-10-11", "2019-10-14 06:00:00")]
        [InlineData("Market4.3", false, "2019-10-08", "2019-10-11 12:00:00")]
        public async Task TestGetMarketsInfo(string marketId, bool isTradingEnabled, 
            string lastTradingDay, string nextTradingDay)
        {
            var repoData = GetPopulatedRepository();

            await TestMarket(marketId, isTradingEnabled, lastTradingDay, nextTradingDay, repoData);
        }
        
        [Theory]
        [InlineData("Market5.1", true, "2019-10-11", "2019-10-13 06:00:00")]
        [InlineData("Market5.2", true, "2019-10-11", "2019-10-14 06:00:00")]
        [InlineData("Market5.3", false, "2019-10-10", "2019-10-11 21:00:00")]
        [InlineData("Market6.1", true, "2019-10-11", "2019-10-17 06:00:00")]
        public async Task TestGetMarketsInfoWithPlatform(string marketId, bool isTradingEnabled, 
            string lastTradingDay, string nextTradingDay)
        {
            var repoData = GetPopulatedRepositoryWithPlatform();

            await TestMarket(marketId, isTradingEnabled, lastTradingDay, nextTradingDay, repoData);
        }

        private static async Task TestMarket(string marketId, bool isTradingEnabled, string lastTradingDay,
            string nextTradingDay, List<IScheduleSettings> repoData)
        {
            var brokerId = Guid.NewGuid().ToString();
            var scheduleServiceMock = new Mock<IScheduleSettingsService>();
            scheduleServiceMock.Setup(r => r.GetFilteredAsync(null)).ReturnsAsync(repoData);

            var systemClockMock = new Mock<ISystemClock>();
            systemClockMock.SetupGet(c => c.UtcNow).Returns(new DateTimeOffset(2019, 10, 11, 11, 0, 0, TimeSpan.Zero));

            var brokerSettingsMock = new Mock<IBrokerSettingsApi>();
            brokerSettingsMock
                .Setup(s => s.GetByIdAsync(It.Is<string>(f => f == brokerId)))
                .ReturnsAsync(
                    new GetBrokerSettingsByIdResponse
                    {
                        ErrorCode = BrokerSettingsErrorCodesContract.None,
                        BrokerSettings = new BrokerSettingsContract
                        {
                            Holidays = new List<DateTime>(),
                            Weekends = new List<DayOfWeek> {DayOfWeek.Saturday, DayOfWeek.Sunday}
                        }
                    });
            
            var service = new MarketDayOffService(scheduleServiceMock.Object, systemClockMock.Object, new PlatformSettings(), brokerId, brokerSettingsMock.Object);

            var info = (await service.GetMarketsInfo(new[] {marketId}, null))[marketId];
            
            brokerSettingsMock.Verify();
            Assert.Equal(isTradingEnabled, info.IsTradingEnabled);
            Assert.Equal(DateTime.Parse(lastTradingDay), info.LastTradingDay);
            Assert.Equal(DateTime.Parse(nextTradingDay).Date, info.NextTradingDayStart.Date);
            Assert.Equal(DateTime.Parse(nextTradingDay).TimeOfDay, info.NextTradingDayStart.TimeOfDay);
        }

        private List<IScheduleSettings> GetPopulatedRepository()
        {
            var repoData = new List<IScheduleSettings>();

            AddSettings(
                repoData,
                "Market1.1",
                new ScheduleConstraint
                {
                    Time = new TimeSpan(1, 0, 0)
                },
                new ScheduleConstraint
                {
                    Time = new TimeSpan(23, 0, 0)
                });
            
            AddSettings(
                repoData,
                "Market1.2",
                new ScheduleConstraint
                {
                    Time = new TimeSpan(12, 0, 0)
                },
                new ScheduleConstraint
                {
                    Time = new TimeSpan(2, 0, 0)
                });
            
            AddSettings(
                repoData,
                "Market1.3",
                new ScheduleConstraint
                {
                    Time = new TimeSpan(01, 0, 0)
                },
                new ScheduleConstraint
                {
                    Time = new TimeSpan(23, 0, 0)
                },
                enabled: true);

            AddSettings(
                repoData,
                "Market2.1",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Monday,
                });
            
            AddSettings(
                repoData,
                "Market2.2",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Friday,
                    Time = new TimeSpan(2, 0, 0)
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                    Time = new TimeSpan(1, 0, 0)
                });
            
            AddSettings(
                repoData,
                "Market2.3",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Monday
                });
            
            AddSettings(
                repoData,
                "Market2.4",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Friday,
                    Time = new TimeSpan(0, 0, 0)
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Friday,
                    Time = new TimeSpan(22, 0, 0)
                });
            
            AddSettings(
                repoData,
                "Market2.5",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Friday,
                    Time = new TimeSpan(22, 0, 0)
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                    Time = new TimeSpan(0, 0, 0)
                });
            
            AddSettings(
                repoData,
                "Market2.6",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Wednesday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Thursday,
                });
            
            AddSettings(
                repoData,
                "Market3.1",
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-11")
                },
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-12")
                });
            
            AddSettings(
                repoData,
                "Market3.2",
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-11")
                },
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-13")
                });
            
            AddSettings(
                repoData,
                "Market3.3",
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-11")
                },
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-15")
                });
            
            AddSettings(
                repoData,
                "Market4.1",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Monday
                });
            
            AddSettings(
                repoData,
                "Market4.1",
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-14")
                },
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-15")
                });
            
            AddSettings(
                repoData,
                "Market4.2",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Monday
                });
            
            AddSettings(
                repoData,
                "Market4.2",
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("22:00:00"),
                },
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("06:00:00"),
                });
            
            AddSettings(
                repoData,
                "Market4.3",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Wednesday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Friday
                });
            
            AddSettings(
                repoData,
                "Market4.3",
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("22:00:00"),
                },
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("12:00:00"),
                });

            return repoData;
        }

        private List<IScheduleSettings> GetPopulatedRepositoryWithPlatform()
        {
            var repoData = new List<IScheduleSettings>();
            
            AddSettings(
                repoData,
                "PlatformScheduleMarketId",
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("22:00:00"),
                },
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("06:00:00"),
                });

            AddSettings(
                repoData,
                "Market5.1",
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-12")
                },
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-13")
                });

            AddSettings(
                repoData,
                "Market5.2",
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Saturday,
                },
                new ScheduleConstraint
                {
                    DayOfWeek = DayOfWeek.Monday
                });

            AddSettings(
                repoData,
                "Market5.2",
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("17:00:00"),
                },
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("23:00:00"),
                });
            
            AddSettings(
                repoData,
                "Market5.3",
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("05:00:00"),
                },
                new ScheduleConstraint
                {
                    Time = TimeSpan.Parse("21:00:00"),
                });
            
            AddSettings(
                repoData,
                "Market6.1",
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-12"),
                },
                new ScheduleConstraint
                {
                    Date = DateTime.Parse("2019-10-17")
                });

            return repoData;
        }

        private void AddSettings(List<IScheduleSettings> data, string marketId, ScheduleConstraint start, 
        ScheduleConstraint end, int rank = 0, bool enabled = false)
        {
            var settings = new ScheduleSettings(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), rank, "*", new HashSet<string>(0), marketId, enabled,
                TimeSpan.Zero, start, end);
            
            data.Add(settings);
        }    
    }
}
