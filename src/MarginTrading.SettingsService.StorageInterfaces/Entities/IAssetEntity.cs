namespace MarginTrading.SettingsService.StorageInterfaces.Entities
{
    public interface IAssetEntity
    {
        string Id { get; }
        string Name { get; }
        int Accuracy { get; }
    }
}
