using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class UpdateCurrencyRequest : UserRequest
    {
        [Required]
        public string InterestRateMdsCode  { get; set; }
    }
}