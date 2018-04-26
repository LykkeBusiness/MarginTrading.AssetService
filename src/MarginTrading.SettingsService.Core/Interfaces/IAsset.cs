namespace MarginTrading.SettingsService.Core.Interfaces
{
    public interface IAsset
    {
        string Id { get; }
        string Name { get; }
        int Accuracy { get; }
    }
}
