using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using MarginTrading.AssetService.Contracts.LegacyAsset;
using MarginTrading.AssetService.Core.Caches;
using MarginTrading.AssetService.Core.Domain;
using Asset = MarginTrading.AssetService.Contracts.LegacyAsset.Asset;

namespace MarginTrading.AssetService.Services.Extensions
{
    public static class AssetExtensions
    {
        public const string DateFormat = "dd/MM/yyyy";
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
            asset.Underlying.RicCode = product.PublicationRic;
            asset.Underlying.HedgeCost = product.HedgeCost;
        }

        public static void SetAssetFieldsFromUnderlying(this Asset asset, UnderlyingsCacheModel underlying)
        {
            asset.Underlying.AlmParam = underlying.AlmParam;
            asset.Underlying.CfiCode = underlying.CfiCode;
            asset.Underlying.Eligible871m = underlying.Eligible871M;
            asset.Underlying.Isin = underlying.Isin;
            asset.Underlying.MdsCode = underlying.MdsCode;
            asset.Underlying.Name = underlying.Name;
            asset.Underlying.RepoSurchargePercent = underlying.RepoSurchargePercent;
            asset.Underlying.Spread = underlying.Spread;
            asset.Underlying.TradingCurrency = underlying.TradingCurrency;
            asset.Underlying.CommodityBase = underlying.CommodityBase;
            asset.Underlying.BaseCurrency = underlying.BaseCurrency;
            asset.Underlying.IndexName = underlying.IndexName;
            asset.Underlying.EMIRType = underlying.EmirType;
            asset.Underlying.CommodityDetails = underlying.CommodityDetails;
            asset.LastTradingDate = underlying.LastTradingDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
            asset.ExpiryDate = underlying.ExpiryDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
            asset.Underlying.MaturityDate = underlying.MaturityDate?.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        public static void SetAssetFieldsFromMarketSettings(this Asset asset, MarketSettings marketSettings)
        {
            var marketScheduleUtcRespectful = marketSettings.MarketSchedule.ShiftToUtc();
            asset.Underlying.MarketDetails.Calendar.Holidays = marketSettings.Holidays;
            asset.Underlying.MarketDetails.MarketHours.Open = marketScheduleUtcRespectful.Open;
            asset.Underlying.MarketDetails.MarketHours.Close = marketScheduleUtcRespectful.Close;
            asset.Underlying.MarketDetails.Name = marketSettings.Name;
            asset.Underlying.MarketDetails.MarketId = marketSettings.Id;
            asset.SetHalfWorkingDays(marketSettings);
        }

        private static void SetHalfWorkingDays(this Asset asset, MarketSettings marketSettings)
        {
            asset.Underlying.MarketDetails.Calendar.HalfWorkingDays = marketSettings.MarketSchedule.HalfWorkingDays.ToList();
        }

        public static void SetDividendFactorFields(this Asset asset, MarketSettings marketSettings,
            BrokerSettingsContract brokerSettings, Product product)
        {
            var dividendShort = product.DividendsShort ?? marketSettings.DividendsShort ?? brokerSettings.DividendsShortPercent;
            var dividendLong = product.DividendsLong ?? marketSettings.DividendsLong ?? brokerSettings.DividendsLongPercent;
            var dividend871M = product.Dividends871M ?? marketSettings.Dividends871M ?? brokerSettings.Dividends871MPercent;
            
            asset.DividendsFactor.Percent = dividendLong;
            asset.DividendsFactor.ShortPercent = dividendShort;
            asset.DividendsFactor.Us871Percent = dividend871M;
            asset.Underlying.MarketDetails.DividendsFactor.Percent = dividendLong;
            asset.Underlying.MarketDetails.DividendsFactor.ShortPercent = dividendShort;
            asset.Underlying.MarketDetails.DividendsFactor.Us871Percent = dividend871M;
            asset.Underlying.DividendsFactor.Percent = dividendLong;
            asset.Underlying.DividendsFactor.ShortPercent = dividendShort;
            asset.Underlying.DividendsFactor.Us871Percent = dividend871M;
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

        public static void SetMargin(this Asset asset, Product product, decimal profileMargin)
        {
            asset.Underlying.MarginRate = product.GetMarginRate(profileMargin);
        }

        public static void SetAssetFieldsFromAssetType(this Asset asset, AssetType assetType)
        {
            asset.Underlying.UnderlyingCategoryId = assetType.UnderlyingCategoryId;
        }
        
        public static void SetAssetFieldsFromClientProfileSettings(this Asset asset, ClientProfileSettings clientProfileSettings)
        {
            asset.Underlying.ExecutionFeeParameter.AssetType = clientProfileSettings.AssetTypeId;
            asset.Underlying.ExecutionFeeParameter.CommissionCap = clientProfileSettings.ExecutionFeesCap;
            asset.Underlying.ExecutionFeeParameter.CommissionFloor = clientProfileSettings.ExecutionFeesFloor;
            asset.Underlying.ExecutionFeeParameter.RatePercent = clientProfileSettings.ExecutionFeesRate / 100;
            asset.Underlying.FinancingFixRate = clientProfileSettings.FinancingFeesRate / 100;
            asset.IsAvailable = clientProfileSettings.IsAvailable;
        }
    }
}