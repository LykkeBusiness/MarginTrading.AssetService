using System.ComponentModel.DataAnnotations;
using MarginTrading.AssetService.Contracts.Core;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class AddProductCategoryRequest : UserRequest
    {
        [Required]
        public string Category { get; set; }
    }
}