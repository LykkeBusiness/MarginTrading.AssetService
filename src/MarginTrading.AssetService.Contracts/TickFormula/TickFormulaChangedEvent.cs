using System;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Enums;
using MessagePack;

namespace MarginTrading.AssetService.Contracts.TickFormula
{
    [MessagePackObject]
    public class TickFormulaChangedEvent
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
        public TickFormulaContract OldTickFormula { get; set; }
        [Key(5)]
        [CanBeNull]
        public TickFormulaContract NewTickFormula { get; set; }
        [Key(6)]
        public ChangeType ChangeType { get; set; }
    }
}