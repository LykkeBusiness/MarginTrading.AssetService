namespace MarginTrading.AssetService.Contracts.LegacyAsset
{
    public static class LegacyAssetExtensions
    {
        public static bool MdsCodeChanged(this AssetUpsertedEvent src)
        {
            var value = src?.PropertiesPriorValueIfUpdated?.UnderlyingMdsCode;
            
            return value != null;
        }
    }
}