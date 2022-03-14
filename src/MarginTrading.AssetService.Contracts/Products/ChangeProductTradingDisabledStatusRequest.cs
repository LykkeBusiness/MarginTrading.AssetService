using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class ChangeProductTradingDisabledStatusRequest : UserRequest
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        public bool TradingDisabled { get; set; }
    }
}