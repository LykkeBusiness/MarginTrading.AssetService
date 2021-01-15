﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Cronut.Dto.Assets;
using Lykke.Snow.Common.Extensions;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using TimeZoneConverter;
using Asset = Cronut.Dto.Assets.Asset;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class CronutAssetExtensions
    {
        public static void SetAssetFieldsFromProduct(this Asset asset, Product product)
        {
            asset.AssetId = product.ProductId;
            asset.UnderlyingMdsCode = product.UnderlyingMdsCode;
            asset.ForceId = product.ForceId;
            asset.Name = product.Name;
            asset.ShortPosition = product.ShortPosition;
            asset.Comments = product.Comments;
            asset.MarketMakerAssetAccountId = product.MarketMakerAssetAccountId;
            asset.IsinLong = product.IsinLong;
            asset.IsinShort = product.IsinShort;
            asset.Issuer = product.Issuer;
            asset.MaxPositionSize = product.MaxPositionSize;
            asset.SettlementCurrency = product.SettlementCurrency;
            asset.MaxOrderSize = product.MaxOrderSize;
            asset.NewsId = product.NewsId;
            asset.ContractSize = product.ContractSize;
            asset.MinOrderDistancePercent = product.MinOrderDistancePercent;
            asset.MinOrderEntryInterval = product.MinOrderEntryInterval;
            asset.MinOrderSize = product.MinOrderSize;
            asset.Parity = product.Parity;
            asset.PublicationRic = product.PublicationRic;
            asset.Tags = product.Tags;
            asset.TickFormulaName = product.TickFormula;
            asset.Parity = product.Parity;
            asset.OvernightMarginMultiplier = product.OvernightMarginMultiplier;
            asset.Underlying.AssetType = product.AssetType;
            asset.Underlying.Keywords = product.Keywords;
            asset.LiquidationThresholdQuantity = 0;
        }

        public static void SetAssetFieldsFromUnderlying(this Asset asset, UnderlyingsCacheModel underlying)
        {
            const string dateFormat = "dd/MM/yyyy";
            asset.Underlying.AlmParam = underlying.AlmParam;
            asset.Underlying.CfiCode = underlying.CfiCode;
            asset.Underlying.Eligible871m = underlying.Eligible871M;
            asset.Underlying.HedgeCost = underlying.HedgeCost;
            asset.Underlying.Isin = underlying.Isin;
            asset.Underlying.MdsCode = underlying.MdsCode;
            asset.Underlying.Name = underlying.Name;
            asset.Underlying.RepoSurchargePercent = underlying.RepoSurchargePercent;
            asset.Underlying.RicCode = underlying.RicCode;
            asset.Underlying.Spread = underlying.Spread;
            asset.Underlying.TradingCurrency = underlying.TradingCurrency;
            asset.Underlying.CommodityBase = underlying.CommodityBase;
            asset.Underlying.BaseCurrency = underlying.BaseCurrency;
            asset.Underlying.IndexName = underlying.IndexName;
            asset.Underlying.EMIRType = underlying.EmirType;
            asset.Underlying.CommodityDetails = underlying.CommodityDetails;
            asset.LastTradingDate = underlying.LastTradingDate?.ToString(dateFormat, CultureInfo.InvariantCulture);
            asset.ExpiryDate = underlying.ExpiryDate?.ToString(dateFormat, CultureInfo.InvariantCulture);
            asset.Underlying.MaturityDate = underlying.MaturityDate?.ToString(dateFormat, CultureInfo.InvariantCulture);
        }

        public static void SetAssetFieldsFromMarketSettings(this Asset asset, MarketSettings marketSettings)
        {
            var timezoneInfo = TZConvert.GetTimeZoneInfo(marketSettings.Timezone);
            var openUtc = marketSettings.Open.ShiftToUtc(timezoneInfo);
            var closeUtc = marketSettings.Close.ShiftToUtc(timezoneInfo);
            //Maybe we can have more than a day after we apply timezone, so we need to remove day portion
            var openUtcWithoutDays = new TimeSpan(openUtc.Hours, openUtc.Minutes, openUtc.Seconds);
            var closeUtcWithoutDays = new TimeSpan(closeUtc.Hours, closeUtc.Minutes, closeUtc.Seconds);

            asset.Underlying.MarketDetails.Calendar.Holidays = marketSettings.Holidays;
            asset.Underlying.MarketDetails.MarketHours.Open = openUtcWithoutDays;
            asset.Underlying.MarketDetails.MarketHours.Close = closeUtcWithoutDays;
            asset.Underlying.MarketDetails.Name = marketSettings.Name;
            asset.Underlying.MarketDetails.MarketId = marketSettings.Id;
            asset.Underlying.MarketName = marketSettings.Name;
            asset.DividendsFactor.Percent = marketSettings.DividendsLong;
            asset.DividendsFactor.ShortPercent = marketSettings.DividendsShort;
            asset.DividendsFactor.Us871Percent = marketSettings.Dividends871M;
            asset.Underlying.MarketDetails.DividendsFactor.Percent = marketSettings.DividendsLong;
            asset.Underlying.MarketDetails.DividendsFactor.ShortPercent = marketSettings.DividendsShort;
            asset.Underlying.MarketDetails.DividendsFactor.Us871Percent = marketSettings.Dividends871M;
            asset.Underlying.DividendsFactor.Percent = marketSettings.DividendsLong;
            asset.Underlying.DividendsFactor.ShortPercent = marketSettings.DividendsShort;
            asset.Underlying.DividendsFactor.Us871Percent = marketSettings.Dividends871M;
        }

        public static void SetAssetFieldsFromTickFormula(this Asset asset, ITickFormula tickFormula)
        {
            asset.TickFormulaName = tickFormula.Id;
            asset.TickFormulaDetails.Name = tickFormula.Id;
            asset.TickFormulaDetails.TickFormulaParameters.Ladders = tickFormula.PdlLadders;
            asset.TickFormulaDetails.TickFormulaParameters.Values = tickFormula.PdlTicks;
        }

        public static void SetAssetFieldsFromTradingCurrency(this Asset asset, 
            Currency tradingCurrency, 
            IList<string> assetTypesWithZeroInterestRate)
        {
            asset.SetAssetFieldsFromCurrency(tradingCurrency, 
                assetTypesWithZeroInterestRate,
                x => asset.Underlying.VariableInterestRate1 = x);
        }

        public static void SetAssetFieldsFromBaseCurrency(this Asset asset, 
            Currency baseCurrency, 
            IList<string> assetTypesWithZeroInterestRate)
        {
            asset.SetAssetFieldsFromCurrency(baseCurrency,
                assetTypesWithZeroInterestRate,
                x => asset.Underlying.VariableInterestRate2 = x);
        }

        private static void SetAssetFieldsFromCurrency(this Asset asset,
            Currency currency,
            IList<string> assetTypesWithZeroInterestRate,
            Action<string> assignInterestRate)
        {
            if (assetTypesWithZeroInterestRate.Contains(asset.Underlying.AssetType) || currency == null)
            {
                assignInterestRate(string.Empty);
                return;
            }

            assignInterestRate(currency.InterestRateMdsCode);
            asset.Underlying.InterestRates.Add(new InterestRate
            {
                MdsCode = currency.InterestRateMdsCode,
                Currency = currency.Id,
                Name = currency.InterestRateMdsCode,
            });
        }

        public static void SetAssetFieldsFromCategory(this Asset asset, ProductCategory category)
        {
            asset.CategoryRaw = category.Id;
        }

        public static void SetAssetFieldsFromClientProfileSettings(this Asset asset, ClientProfileSettings clientProfileSettings)
        {
            asset.Underlying.ExecutionFeeParameter.Currency = "EUR";
            asset.Underlying.ExecutionFeeParameter.AssetType = clientProfileSettings.AssetTypeId;
            asset.Underlying.ExecutionFeeParameter.CommissionCap = clientProfileSettings.ExecutionFeesCap;
            asset.Underlying.ExecutionFeeParameter.CommissionFloor = clientProfileSettings.ExecutionFeesFloor;
            asset.Underlying.ExecutionFeeParameter.RatePercent = clientProfileSettings.ExecutionFeesRate / 100;
            asset.Underlying.MarginRate = clientProfileSettings.Margin / 100;
            asset.Underlying.FinancingFixRate = clientProfileSettings.FinancingFeesRate / 100;
        }
    }
}