using System;

namespace MarginTrading.AssetService.Contracts.Products
{
    public class ProductFreezeInfoContract
    {
        public ProductFreezeReasonContract Reason { get; set; }

        public string Comment { get; set; }

        public DateTime? UnfreezeDate { get; set; }
    }
}