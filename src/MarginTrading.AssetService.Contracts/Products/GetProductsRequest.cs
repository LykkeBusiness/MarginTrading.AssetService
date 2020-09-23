namespace MarginTrading.AssetService.Contracts.Products
{
    public class GetProductsRequest
    {
        public string[] MdsCodes { get; set; }

        public string[] ProductIds { get; set; }
    }
}