namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    public interface IClientProfileCache
    {
        void AddOrUpdate(ClientProfileContract clientProfile);
        void Remove(ClientProfileContract clientProfile);
    }
}