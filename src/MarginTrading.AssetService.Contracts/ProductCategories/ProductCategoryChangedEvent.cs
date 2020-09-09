using System;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.ProductCategories
{
    [MessagePackObject]
    public class ProductCategoryChangedEvent
    {
        [Key(0)]
        public string Username { get; set; }
        [Key(1)]
        public string CorrelationId { get; set; }
        [Key(2)]
        public string EventId { get; set; }
        [Key(3)]
        public DateTime Timestamp { get; set; }
        [Key(4)]
        [CanBeNull]
        public ProductCategoryContract OldProductCategory { get; set; }
        [Key(5)]
        [CanBeNull]
        public ProductCategoryContract NewProductCategory { get; set; }
        [Key(6)]
        public ChangeType ChangeType { get; set; }
        /// <summary>
        /// Unformatted category name, only filled on create events
        /// </summary>
        [Key(7)]
        [CanBeNull]
        public string OriginalCategoryName { get; set; }
    }
}