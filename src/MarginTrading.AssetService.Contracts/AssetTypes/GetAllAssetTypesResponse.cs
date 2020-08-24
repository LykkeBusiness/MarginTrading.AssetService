using System.Collections.Generic;

namespace MarginTrading.AssetService.Contracts.AssetTypes
{
    public class GetAllAssetTypesResponse
    {
        public IReadOnlyList<AssetTypeContract> AssetTypes { get; set; }
    }
}