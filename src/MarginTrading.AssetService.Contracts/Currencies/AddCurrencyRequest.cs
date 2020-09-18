using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    public class AddCurrencyRequest : UserRequest
    {
        [Required]
        [MaxLength(100)]
        public string Id { get; set; }
        [Required]
        public string InterestRateMdsCode  { get; set; }
    }
}