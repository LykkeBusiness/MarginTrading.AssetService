using System;
using System.Collections.Generic;
using System.Linq;
using MarginTrading.AssetService.Contracts.LegacyAsset;

namespace MarginTrading.AssetService.Mappers
{
    public static class LegacyAssetMappers
    {
        public static LegacyAssetContract ToContract(this Cronut.Dto.Assets.Asset source)
        {
            return new LegacyAssetContract
            {
                AssetId = source.AssetId,
                CategoryBottomLevel = source.CategoryBottomLevel,
                CategoryRaw = source.CategoryRaw,
                CategoryTopLevel = source.CategoryTopLevel,
                Comments = source.Comments,
                ContractSize = source.ContractSize,
                DisplayPrecision = source.DisplayPrecision,
                DividendsFactor = Map(source.DividendsFactor),
                ExpiryDate = source.ExpiryDate,
                ShortPosition = source.ShortPosition,
                ForceId = source.ForceId,
                IsinLong = source.IsinLong,
                IsinShort = source.IsinShort,
                Issuer = source.Issuer,
                LastTradingDate = source.LastTradingDate,
                LiquidationThresholdQuantity = source.LiquidationThresholdQuantity,
                MarketMakerAssetAccountId = source.MarketMakerAssetAccountId,
                MaxOrderSize = source.MaxOrderSize,
                MaxPositionSize = source.MaxPositionSize,
                MinOrderDistancePercent = source.MinOrderDistancePercent,
                MinOrderEntryInterval = source.MinOrderEntryInterval,
                MinOrderSize = source.MinOrderSize,
                Name = source.Name,
                SettlementCurrency = source.SettlementCurrency,
                NewsId = source.NewsId,
                OvernightMarginMultiplier = source.OvernightMarginMultiplier,
                Parity = source.Parity,
                PublicationRic = source.PublicationRic,
                Tags = source.Tags,
                TickFormulaDetails = Map(source.TickFormulaDetails),
                TickFormulaName = source.TickFormulaName,
                Underlying = Map(source.Underlying),
                UnderlyingMdsCode = source.UnderlyingMdsCode
            };
        }


        public static IReadOnlyCollection<LegacyAssetContract> ToContract(this IReadOnlyCollection<Cronut.Dto.Assets.Asset> source)
        {
            return source.Select(ToContract).ToList();
        }

        private static DividendsFactor Map(Cronut.Dto.Assets.DividendsFactor source)
        {
            if (source == null)
            {
                return null;
            }

            return new DividendsFactor
            {
                Percent = source.Percent,
                ShortPercent = source.ShortPercent,
                Us871Percent = source.Us871Percent
            };
        }

        private static TickFormula Map(Cronut.Dto.Assets.TickFormula source)
        {
            if (source == null)
            {
                return null;
            }

            return new TickFormula
            {
                Name = source.Name,
                TickFormulaParameters = Map(source.TickFormulaParameters)
            };
        }

        private static Underlying Map(Cronut.Dto.Assets.Underlying source)
        {
            if (source == null)
            {
                return null;
            }

            return new Underlying
            {
                AlmParam = source.AlmParam,
                DividendsFactor = Map(source.DividendsFactor),
                Name = source.Name,
                AssetType = source.AssetType,
                BaseCurrency = source.BaseCurrency,
                CfiCode = source.CfiCode,
                CommodityBase = source.CommodityBase,
                CommodityDetails = source.CommodityDetails,
                EMIRType = source.EMIRType,
                Eligible871m = source.Eligible871m,
                ExecutionFeeParameter = Map(source.ExecutionFeeParameter),
                FinancingFixRate = source.FinancingFixRate,
                HedgeCost = source.HedgeCost,
                IndexName = source.IndexName,
                InterestRates = source.InterestRates.Select(Map).ToList(),
                Isin = source.Isin,
                Keywords = source.Keywords,
                MarginRate = source.MarginRate,
                MarketDetails = Map(source.MarketDetails),
                MarketId = source.MarketName,
                MaturityDate = source.MaturityDate,
                MdsCode = source.MdsCode,
                RepoSurchargePercent = source.RepoSurchargePercent,
                RicCode = source.RicCode,
                Spread = source.Spread,
                TradingCurrency = source.TradingCurrency,
                VariableInterestRate = source.VariableInterestRate,
                VariableInterestRate1 = source.VariableInterestRate1,
                VariableInterestRate2 = source.VariableInterestRate2
            };
        }

        private static TickFormulaParameters Map(Cronut.Dto.Assets.TickFormulaParameters source)
        {
            if (source == null)
            {
                return null;
            }

            return new TickFormulaParameters
            {
                Ladders = source.Ladders,
                Values = source.Values
            };
        }

        private static ExecutionFeeParameter Map(Cronut.Dto.Assets.ExecutionFeeParameter source)
        {
            if (source == null)
            {
                return null;
            }

            return new ExecutionFeeParameter
            {
                AssetType = source.AssetType,
                CommissionCap = source.CommissionCap,
                CommissionFloor = source.CommissionFloor,
                Currency = source.Currency,
                RatePercent = source.RatePercent
            };
        }

        private static InterestRate Map(Cronut.Dto.Assets.InterestRate source)
        {
            if (source == null)
            {
                return null;
            }

            return new InterestRate
            {
                Name = source.Name,
                Currency = source.Currency,
                MdsCode = source.MdsCode,
            };
        }

        private static Market Map(Cronut.Dto.Assets.Market source)
        {
            if (source == null)
            {
                return null;
            }

            return new Market
            {
                DividendsFactor = Map(source.DividendsFactor),
                MarketId = source.MarketId,
                Name = source.Name,
                Calendar = Map(source.Calendar),
                MarketHours = Map(source.MarketHours)
            };
        }

        private static Calendar Map(Cronut.Dto.Assets.Calendar source)
        {
            if (source == null)
            {
                return null;
            }

            return new Calendar
            {
                Holidays = source.Holidays
            };
        }

        private static MarketHours Map(Cronut.Dto.Assets.MarketHours source)
        {
            if (source == null)
            {
                return null;
            }

            return new MarketHours
            {
                Close = source.Close,
                Day = source.Day,
                Open = source.Open
            };
        }
    }
}
