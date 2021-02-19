namespace MarginTrading.AssetService.Contracts.Products
{
    public class ChangeMultipleProductFrozenStatusRequest
    {
        public string[] ProductIds { get; set; }
        
        public ChangeProductFrozenStatusRequest FreezeParameters { get; set; } 
    }
}