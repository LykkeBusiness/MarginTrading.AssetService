// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using Lykke.Snow.Common.WorkingDays;

using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Extensions;

namespace MarginTrading.AssetService.Core.Domain
{

    public static class MarketSettingsFactory
    {
        public static MarketSettings FromRequest(MarketSettingsCreateOrUpdateDto model)
        {
            var (schedule, errorCode) = TryCreateMarketSchedule(model);
            var noError = errorCode == MarketSettingsErrorCodes.None;
            return noError
                ? FromRequestAndSchedule(model, schedule)
                : new InvalidMarketSettings(errorCode);
        }

        private static MarketSettings FromRequestAndSchedule(MarketSettingsCreateOrUpdateDto model,
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

        private static (MarketSchedule, MarketSettingsErrorCodes) TryCreateMarketSchedule(
            MarketSettingsCreateOrUpdateDto model)
        {
            MarketSchedule schedule = null;
            var errorCode = MarketSettingsErrorCodes.None;
            try
            {
                var open = model.Open.GetValueOrDefault(MarketSettingsConstants.DefaultOpen);
                var close = model.Close.GetValueOrDefault(MarketSettingsConstants.DefaultClose);
                var timeZoneId = GetValueOrDefault(model.Timezone, MarketSettingsConstants.DefaultTimeZone);
                schedule = new MarketSchedule(open, close, timeZoneId, model.HalfWorkingDays);
            }
            catch (Exception e)
            {
                if (!MarketScheduleExceptionToErrorCode.Map.TryGetValue(e.GetType(), out errorCode))
                    throw;
            }

            return (schedule, errorCode);
        }

        private static string GetValueOrDefault(string value, string defaultValue) =>
            string.IsNullOrEmpty(value) ? defaultValue : value;
    }
}