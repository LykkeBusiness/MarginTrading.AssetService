﻿using System;
using System.Collections.Generic;

using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Common.WorkingDays;

using Newtonsoft.Json;

using TimeZoneConverter;

namespace MarginTrading.AssetService.Core.Domain
{
    public class MarketSettings : IAuditableObject<AuditDataType>
    {
        private const int DividendsLongMaxValue = 200;
        private const int DividendsShortMaxValue = 200;
        private const int Dividends871MMaxValue = 100;
        
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? DividendsLong { get; set; }

        public decimal? DividendsShort { get; set; }

        public decimal? Dividends871M { get; set; }

        public List<DateTime> Holidays { get; set; }
        
        public MarketSchedule MarketSchedule { get; set; }
        
        public AuditDataType GetAuditDataType() => AuditDataType.MarketSettings;

        public string GetAuditReference() => Id;

        public string ToAuditJson() =>
            JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Converters = new List<JsonConverter> { new WorkingDayConverter() }
                });

        public Result<MarketSettingsErrorCodes> Validate()
        {
            if (DividendsLong < 0 || DividendsLong > DividendsLongMaxValue)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividendsLongValue);

            if (DividendsShort < 0 || DividendsShort > DividendsShortMaxValue)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividendsShortValue);

            if (Dividends871M < 0 || Dividends871M > Dividends871MMaxValue)
                return new Result<MarketSettingsErrorCodes>(MarketSettingsErrorCodes.InvalidDividends871MValue);

            return new Result<MarketSettingsErrorCodes>();
        }

        public DateTime ConvertToMarketTimeZone(DateTime dateTime)
        {
            var timeZone = TZConvert.GetTimeZoneInfo(MarketSchedule.TimeZoneId);
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }
    }
}