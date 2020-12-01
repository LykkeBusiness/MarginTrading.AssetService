// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using MarginTrading.AssetService.Core.Constants;
using MarginTrading.AssetService.Core.Interfaces;

namespace MarginTrading.AssetService.Core.Domain
{
    public class AssetPair : IAssetPair
    {
        public AssetPair(string id, string name, string baseAssetId, string quoteAssetId, int accuracy, string marketId,
            string legalEntity, string basePairId, MatchingEngineMode matchingEngineMode,
            decimal stpMultiplierMarkupBid, decimal stpMultiplierMarkupAsk,
            bool isSuspended, bool isFrozen, bool isDiscontinued, FreezeInfo freezeInfo, string assetType)
        {
            Id = id;
            Name = name;
            BaseAssetId = baseAssetId;
            QuoteAssetId = quoteAssetId;
            Accuracy = accuracy;
            MarketId = marketId;
            LegalEntity = legalEntity;
            BasePairId = basePairId;
            MatchingEngineMode = matchingEngineMode;
            StpMultiplierMarkupBid = stpMultiplierMarkupBid;
            StpMultiplierMarkupAsk = stpMultiplierMarkupAsk;

            IsSuspended = isSuspended;
            IsFrozen = isFrozen;
            IsDiscontinued = isDiscontinued;
            FreezeInfo = freezeInfo ?? new FreezeInfo();
            AssetType = assetType;
        }

        public string Id { get; }
        public string Name { get; }
        public string BaseAssetId { get; }
        public string QuoteAssetId { get; }
        [Obsolete]
        public int Accuracy { get; }
        public string MarketId { get; }
        public string LegalEntity { get; }
        [Obsolete]
        public string BasePairId { get; }
        [Obsolete]
        public MatchingEngineMode MatchingEngineMode { get; }
        [Obsolete]
        public decimal StpMultiplierMarkupBid { get; }
        [Obsolete]
        public decimal StpMultiplierMarkupAsk { get; }

        public bool IsSuspended { get; }
        public bool IsFrozen { get; }
        public bool IsDiscontinued { get; }
        public FreezeInfo FreezeInfo { get; }
        public string AssetType { get; }

        public static IAssetPair CreateFromProduct(Product product, string legalEntity)
        {
            return new AssetPair(
                id: product.ProductId,
                name: product.Name,
                baseAssetId: product.ProductId,
                quoteAssetId: product.TradingCurrency,
                accuracy: AssetPairConstants.Accuracy,
                marketId: product.Market,
                legalEntity: legalEntity,
                basePairId: AssetPairConstants.BasePairId,
                matchingEngineMode: AssetPairConstants.MatchingEngineMode,
                stpMultiplierMarkupBid: AssetPairConstants.StpMultiplierMarkupBid,
                stpMultiplierMarkupAsk: AssetPairConstants.StpMultiplierMarkupAsk,
                isSuspended: product.IsSuspended,
                isFrozen: product.IsFrozen,
                isDiscontinued: product.IsDiscontinued,
                freezeInfo: new FreezeInfo
                {
                    Comment = product.FreezeInfo.Comment,
                    Reason = (FreezeReason)product.FreezeInfo.Reason,
                    UnfreezeDate = product.FreezeInfo.UnfreezeDate,
                },
                assetType:product.AssetType
            );
        }

        public static IAssetPair CreateFromCurrency(Currency currency, string legalEntity, string baseCurrencyId)
        {
            var id = $"{baseCurrencyId}{currency.Id}";

            return new AssetPair(
                id: id,
                name: id,
                baseAssetId: baseCurrencyId,
                quoteAssetId: currency.Id,
                accuracy: AssetPairConstants.Accuracy,
                marketId: AssetPairConstants.FxMarketId,
                legalEntity: legalEntity,
                basePairId: AssetPairConstants.BasePairId,
                matchingEngineMode: AssetPairConstants.MatchingEngineMode,
                stpMultiplierMarkupBid: AssetPairConstants.StpMultiplierMarkupBid,
                stpMultiplierMarkupAsk: AssetPairConstants.StpMultiplierMarkupAsk,
                isSuspended: false,
                isFrozen: false,
                isDiscontinued: false,
                freezeInfo: new FreezeInfo(),
                assetType: null
                );
        }
    }
}