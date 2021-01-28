;UPDATE [dbo].[MarketSettings]
 SET [dbo].[MarketSettings].[MarketSchedule_Schedule] =
     '{ "Open": ["' + CONVERT(varchar, [Open], 8) + '"], "Close": ["' + CONVERT(varchar, [Close], 8) + '"], "HalfWorkingDays": [], "TimeZoneId": "' + [Timezone] + '" }';