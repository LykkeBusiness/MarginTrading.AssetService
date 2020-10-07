using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Constants
{
    public static class TradingConditionConstants
    {
        public const string LimitCurrency = "EUR";
        public const decimal DepositLimit = 0;
        public const decimal WithdrawalLimit = 0;
        public static readonly List<string> BaseAssets = new List<string> { "EUR" };
    }
}