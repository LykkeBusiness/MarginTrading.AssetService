using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.TradingConditions
{
    public class CheckProductsUnavailableForTradingConditionRequest
    {
        [Required]
        public List<string> ProductIds { get; set; }
        [Required]
        public string TradingConditionId { get; set; }
    }
}