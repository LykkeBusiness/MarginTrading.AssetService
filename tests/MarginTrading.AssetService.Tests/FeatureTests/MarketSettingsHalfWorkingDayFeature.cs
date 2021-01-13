using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Contracts.MarketSettings;
using MessagePack;
using Xunit;

namespace MarginTrading.AssetService.Tests.FeatureTests
{
    public class MarketSettingsHalfWorkingDayFeature
    {
        [Fact]
        public void MarketSettingsContract_MessagePack_Serialization()
        {
            var o = new MarketSettingsContract
            {
                Holidays = new List<DateTime>(),
                MarketSchedule = new MarketScheduleContract
                {
                    HalfWorkingDays = new List<WorkingDay>
                    {
                        new WorkingDay(WorkingDayDuration.WholeDay, DateTime.UtcNow)
                    }
                }
            };
            
            var bin = MessagePackSerializer.Serialize(o);
            var marketSettings = MessagePackSerializer.Deserialize<MarketSettingsContract>(bin);

            var halfWorkingDay = marketSettings.MarketSchedule.HalfWorkingDays.First();
            Assert.Equal(WorkingDayDuration.WholeDay, halfWorkingDay.Duration);
            Assert.Equal(DateTime.UtcNow.Date, halfWorkingDay.Timestamp);
        }
    }
}