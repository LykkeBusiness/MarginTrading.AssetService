using Refit;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsRequest
    {
        [Query(CollectionFormat.Multi)]
        public string[] MdsCodes { get; set; }

        [Query(CollectionFormat.Multi)]
        public string[] ProductIds { get; set; }
    }
}