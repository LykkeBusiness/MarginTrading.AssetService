using System;

namespace MarginTrading.SettingsService.Core
{
    public static class PaginationHelper
    {
        public const int MaxResults = 100;
        public const int UnspecifiedResults = 20;

        public static int GetTake(int? take)
        {
            return take == null
                ? UnspecifiedResults
                : Math.Min(take.Value, MaxResults);
        }
    }
}