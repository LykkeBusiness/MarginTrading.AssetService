using Microsoft.WindowsAzure.Storage.Table;

namespace MarginTrading.SettingsService.AzureRepositories
{
    public class SimpleAzureEntity : TableEntity
    {
        public virtual string Id { get; set; }

        public virtual void SetRowKey()
        {
            this.RowKey = this.Id;
        }
    }
}