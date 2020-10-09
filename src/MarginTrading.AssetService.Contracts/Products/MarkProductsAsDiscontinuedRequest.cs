using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Common;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class MarkProductsAsDiscontinuedRequest : UserRequest
    {
        [Required]
        public string[] ProductIds { get; set; }
    }
}