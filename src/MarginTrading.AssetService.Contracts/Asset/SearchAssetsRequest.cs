using System;

namespace MarginTrading.AssetService.Contracts.Asset
{
    public class SearchAssetsRequest
    {
        /// <summary>
        /// Expiry date of the product from, if product has a end date or a maturity date.
        /// </summary>
        public DateTime? ExpiryDateFrom { get; set; }

        /// <summary>
        /// Expiry date of the product to, if product has a end date or a maturity date.
        /// </summary>
        public DateTime? ExpiryDateTo { get; set; }

        /// <summary>
        /// Ref.file underlyings ISIN
        /// </summary>
        public string UnderlyingIsIn { get; set; }

        /// <summary>
        /// Ref.file underlyings Type
        /// </summary>
        public string UnderlyingType { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// MDS code
        /// </summary>
        public string MdsCode { get; set; }

        /// <summary>
        /// Asset name
        /// </summary>
        public string AssetName { get; set; }
    }
}
