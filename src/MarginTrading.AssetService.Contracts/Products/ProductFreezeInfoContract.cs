using System;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.Products
{
    [MessagePackObject]
    public class ProductFreezeInfoContract
    {
        [Key(0)]
        public ProductFreezeReasonContract Reason { get; set; }

        [Key(1)]
        public string Comment { get; set; }

        [Key(2)]
        public DateTime? UnfreezeDate { get; set; }
    }
}