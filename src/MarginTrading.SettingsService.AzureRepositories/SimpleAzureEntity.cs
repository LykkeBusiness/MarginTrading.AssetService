using Lykke.AzureStorage.Tables;

namespace MarginTrading.SettingsService.AzureRepositories
{
    public class SimpleAzureEntity : AzureTableEntity
    {
        public virtual string Id { get; set; }
        internal virtual string SimplePartitionKey { get; }

        public virtual void SetKeys()
        {
            this.RowKey = this.Id;
            this.PartitionKey = SimplePartitionKey;
        }
    }
}