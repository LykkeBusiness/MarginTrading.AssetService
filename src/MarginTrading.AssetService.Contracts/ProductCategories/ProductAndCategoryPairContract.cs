using System.ComponentModel.DataAnnotations;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    public class ProductAndCategoryPairContract
    {
        [Required]
        [MaxLength(100)]
        public string ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Category { get; set; }
    }
}