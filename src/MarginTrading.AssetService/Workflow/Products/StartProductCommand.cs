using MessagePack;

namespace MarginTrading.AssetService.Workflow.Products
{
    [MessagePackObject]
    public class StartProductCommand
    {
        [Key(0)]
        public string ProductId { get; set; }

        [Key(1)]
        public string OperationId { get; set; }
    }
}