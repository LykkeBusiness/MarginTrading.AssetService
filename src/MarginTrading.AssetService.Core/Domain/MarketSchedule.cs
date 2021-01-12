using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Common.Extensions;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Core.Exceptions;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketSchedule : ScheduleBase<TimeSpan[]>
    {
        public MarketSchedule(TimeSpan[] open,
            TimeSpan[] close,
            string timeZoneId,
            IEnumerable<WorkingDay> halfWorkingDays)
            : this(open, close, timeZoneId, halfWorkingDays.Select(d => d.ToString()))
        {
        }

        public MarketSchedule(TimeSpan[] open, 
            TimeSpan[] close, 
            string timeZoneId, 
            IEnumerable<string> halfWorkingDays)
            : base(open, close, timeZoneId, halfWorkingDays)
        {
            if (open.Length != close.Length)
                throw new  InvalidOpenAndCloseHoursException();

            for (int i = 0; i < open.Length; i++)
            {
                if (open[i].TotalHours >= 24 ||
                    close[i].TotalHours >= 24 ||
                    open[i] > close[i] && close[i] != TimeSpan.Zero)
                    throw new InvalidOpenAndCloseHoursException();

                var openUtc = open[i].ShiftToUtc(TimeZoneInfo);
                var closeUtc = close[i].ShiftToUtc(TimeZoneInfo);

                if (openUtc.TotalHours >= 24 ||
                    closeUtc.TotalHours >= 24 ||
                    openUtc.TotalHours < 0 ||
                    closeUtc.TotalHours < 0)
                    throw new OpenAndCloseWithAppliedTimezoneMustBeInTheSameDayException();

                var firstTradingSession = i == 0;
                if (!firstTradingSession)
                {
                    var previousClose = close[i - 1];
                    if (open[i] <= previousClose)
                        throw new InvalidOpenAndCloseHoursException();
                }
            }

            Open = open;
            Close = close;
        }
        
        public MarketSchedule ShiftToUtc()
        {
            var halfWorkingDaysUtc = _halfWorkingDays.Select(d => d.ShiftToUtc(TimeZoneInfo));

            return new MarketSchedule(
                Open.Select(x => x.ShiftToUtc(TimeZoneInfo)).ToArray(),
                Close.Select(x => x.ShiftToUtc(TimeZoneInfo)).ToArray(),
                TimeZoneInfo.Utc.Id,
                halfWorkingDaysUtc);
        }
    }
}