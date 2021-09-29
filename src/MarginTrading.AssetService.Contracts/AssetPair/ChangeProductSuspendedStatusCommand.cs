using System;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.AssetPair
{
    [MessagePackObject]
    public class ChangeProductSuspendedStatusCommand
    {
        [Key(0)]
        public string OperationId { get; set; }
        
        [Key(1)]
        public string ProductId { get; set; }

        [Key(2)]
        public bool IsSuspended { get; set; }
        
        [Key(3)] 
        public DateTime Timestamp { get; set; }
    }
}