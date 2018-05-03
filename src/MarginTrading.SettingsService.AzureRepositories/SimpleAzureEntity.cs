using Lykke.AzureStorage.Tables;
using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories
{
    public class SimpleAzureEntity : AzureTableEntity
    {
        public virtual string Id { get; protected set; }
        internal virtual string SimplePartitionKey { get; }

        public virtual void SetKeys()
        {
            this.RowKey = this.Id;
            this.PartitionKey = SimplePartitionKey;
        }
    }
}