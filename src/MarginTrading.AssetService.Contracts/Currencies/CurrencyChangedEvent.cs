using System;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.Currencies
{
    [MessagePackObject]
    public class CurrencyChangedEvent
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
        public CurrencyContract OldCurrency { get; set; }
        [Key(5)]
        [CanBeNull]
        public CurrencyContract NewCurrency { get; set; }
        [Key(6)]
        public ChangeType ChangeType { get; set; }
    }
}