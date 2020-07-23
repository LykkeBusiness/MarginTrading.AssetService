// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.AzureStorage.Tables;

namespace MarginTrading.AssetService.AzureRepositories
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