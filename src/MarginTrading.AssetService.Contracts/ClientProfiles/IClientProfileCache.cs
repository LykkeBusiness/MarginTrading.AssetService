namespace MarginTrading.AssetService.Contracts.ClientProfiles
{
    public interface IClientProfileCache
    {
        void Start();
        void AddOrUpdate(ClientProfileContract clientProfile);
        void Remove(ClientProfileContract clientProfile);
    }
}