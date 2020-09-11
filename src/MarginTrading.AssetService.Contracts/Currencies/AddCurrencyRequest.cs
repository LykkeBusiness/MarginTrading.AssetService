using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class AddCurrencyRequest : UserRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string InterestRateMdsCode  { get; set; }
    }
}