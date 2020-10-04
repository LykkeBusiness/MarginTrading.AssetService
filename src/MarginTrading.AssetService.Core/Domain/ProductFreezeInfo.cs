using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ProductFreezeInfo
    {
        public ProductFreezeReason Reason { get; set; }
        
        public string Comment { get; set; }
        
        public DateTime? UnfreezeDate { get; set; }

        public bool IsDefault => Comment == null &&
                               Reason == ProductFreezeReason.Undefined &&
                               UnfreezeDate == null; 
       
    }

}