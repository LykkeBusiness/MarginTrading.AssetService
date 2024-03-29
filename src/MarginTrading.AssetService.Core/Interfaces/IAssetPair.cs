﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.AssetService.Core.Domain;

namespace MarginTrading.AssetService.Core.Interfaces
{
    public interface IAssetPair
    {
        string Id { get; }
        string Name { get; }
        string BaseAssetId { get; }
        string QuoteAssetId { get; }
        int Accuracy { get; }
        string MarketId { get; }
        string LegalEntity { get; }
        string BasePairId { get; }
        MatchingEngineMode MatchingEngineMode { get; }
        decimal StpMultiplierMarkupBid { get; }
        decimal StpMultiplierMarkupAsk { get; }

        bool IsSuspended { get; }
        bool IsFrozen { get; }
        bool IsDiscontinued { get; }
        FreezeInfo FreezeInfo { get; }
        string AssetType { get; }
        public bool IsTradingDisabled { get; }
        public ContractSize ContractSize { get; }
    }
}
