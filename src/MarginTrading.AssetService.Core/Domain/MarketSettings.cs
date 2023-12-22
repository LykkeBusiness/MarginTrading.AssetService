using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Common.Exceptions;
using Lykke.Snow.Common.WorkingDays;
using MarginTrading.AssetService.Core.Constants;
using Newtonsoft.Json;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketSettings : IAuditableObject<AuditDataType>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public List<DateTime> Holidays { get; set; }
        
        public MarketSchedule MarketSchedule { get; set; }

        public static MarketSettings GetMarketSettingsWithDefaults(MarketSettingsCreateOrUpdateDto model)
        {
            var (schedule, errorCode) = TryGetMarketSchedule(model);
            var noError = errorCode == MarketSettingsErrorCodes.None;
            return noError
                ? FromRequestAndSchedule(model, schedule)
                : new InvalidMarketSettings(errorCode);
        }

        public static MarketSettings FromRequestAndSchedule(MarketSettingsCreateOrUpdateDto model,
            MarketSchedule schedule) =>
            new MarketSettings
            {
                Id = model.Id,
                Name = model.Name,
                Dividends871M = model.Dividends871M,
                DividendsLong = model.DividendsLong,
                DividendsShort = model.DividendsShort,
                Holidays = model.Holidays,
                MarketSchedule = schedule
            };
        
        public static (MarketSchedule, MarketSettingsErrorCodes) TryGetMarketSchedule(
            MarketSettingsCreateOrUpdateDto model)
        {
            MarketSchedule schedule = null;
            var errorCode = MarketSettingsErrorCodes.None;
            try
            {
                var open = GetHoursOrDefault(model.Open, MarketSettingsConstants.DefaultOpen);
                var close = GetHoursOrDefault(model.Close, MarketSettingsConstants.DefaultClose);
                var timeZoneId = GetValueOrDefault(model.Timezone, MarketSettingsConstants.DefaultTimeZone);
                schedule = new MarketSchedule(open, close, timeZoneId, model.HalfWorkingDays);
            }
            catch (InvalidOpenAndCloseHoursException)
            {
                errorCode = MarketSettingsErrorCodes.InvalidOpenAndCloseHours;
            }
            catch (WinterOpenAndCloseWithAppliedTimezoneMustBeInTheSameDayException)
            {
                errorCode = MarketSettingsErrorCodes.WinterOpenAndCloseWithAppliedTimezoneMustBeInTheSameDay;
            }
            catch (SummerOpenAndCloseWithAppliedTimezoneMustBeInTheSameDayException)
            {
                errorCode = MarketSettingsErrorCodes.SummerOpenAndCloseWithAppliedTimezoneMustBeInTheSameDay;
            }
            catch (InvalidTimeZoneException)
            {
                errorCode = MarketSettingsErrorCodes.InvalidTimezone;
            }
            catch (InconsistentWorkingCalendarException)
            {
                errorCode = MarketSettingsErrorCodes.InconsistentWorkingCalendar;
            }
            catch (InvalidWorkingDayStringException)
            {
                errorCode = MarketSettingsErrorCodes.InvalidHalfWorkingDayString;
            }
            
            return (schedule, errorCode);
        }
        
        public static TimeSpan[] GetHoursOrDefault(TimeSpan[] hours, TimeSpan defaultValue) =>
            hours.Any() ? hours : new[] { defaultValue };

        public static string GetValueOrDefault(string value, string defaultValue) =>
            string.IsNullOrEmpty(value) ? defaultValue : value;
        
        public AuditDataType GetAuditDataType() => AuditDataType.MarketSettings;

        public string GetAuditReference() => Id;

        public string ToAuditJson() =>
            JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Converters = new List<JsonConverter> { new WorkingDayConverter() }
                });
    }
}