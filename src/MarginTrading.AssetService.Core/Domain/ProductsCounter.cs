using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class ProductsCounter
    {
        public int Counter { get; }
        
        public string[] MdsCodes { get; }
        
        public string[] ProductIds { get; }

        public ProductsCounter(int counter, string[] mdsCodes = null, string[] productsIds = null)
        {
            if (counter < 0)
                throw new ArgumentOutOfRangeException(nameof(counter));
            
            Counter = counter;
            MdsCodes = mdsCodes;
            ProductIds = productsIds;
        }
    }
}