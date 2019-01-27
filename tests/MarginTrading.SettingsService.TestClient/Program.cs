using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;
using MarginTrading.SettingsService.Contracts;
using MarginTrading.SettingsService.Contracts.Asset;
using MarginTrading.SettingsService.Contracts.AssetPair;
using MarginTrading.SettingsService.Contracts.Enums;
using MarginTrading.SettingsService.Contracts.Market;
using MarginTrading.SettingsService.Contracts.Routes;
using MarginTrading.SettingsService.Contracts.Scheduling;
using MarginTrading.SettingsService.Contracts.TradingConditions;
using Newtonsoft.Json;
using Refit;

namespace MarginTrading.SettingsService.TestClient
{
    /// <summary>
    /// Simple way to check api clients are working.
    /// In future this could be turned into a functional testing app.
    /// </summary>
    internal static class Program
    {
        private static int _counter;

        static async Task Main()
        {
            try
            {
                await Run();
            }
            catch (ApiException e)
            {
                var str = e.Content;
                if (str.StartsWith('"'))
                {
                    str = TryDeserializeToString(str);
                }

                Console.WriteLine(str);
                Console.WriteLine(e.ToAsyncString());
            }
        }

        private static string TryDeserializeToString(string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<string>(str);
            }
            catch
            {
                return str;
            }
        }

        private static async Task Run()
        {
            var clientGenerator = HttpClientGenerator.BuildForUrl("http://localhost:5010").Create();

            await CheckAssetPairsApiWorking(clientGenerator);
            await CheckAssetsApiWorking(clientGenerator);
            await CheckMarketsApiWorking(clientGenerator);
            await CheckScheduleSettingsApiWorking(clientGenerator);
            await CheckServiceMaintenanceApiWorking(clientGenerator);
            await CheckTradingConditionsApiWorking(clientGenerator);
            await CheckTradingInstrumentsApiWorking(clientGenerator);
            await CheckTradingRoutesApiWorking(clientGenerator);

            Console.WriteLine("Successfully finished");
        }

        private static async Task CheckAssetPairsApiWorking(IHttpClientGenerator clientGenerator)
        {
            var assetPairContract = new AssetPairContract
            {
                Id = "t1",
                Name = "1",
                BaseAssetId = "1",
                QuoteAssetId = "1",
                Accuracy = 3,
                MarketId = "1",
                LegalEntity = "1",
                BasePairId = "1",
                MatchingEngineMode = MatchingEngineModeContract.MarketMaker,
                StpMultiplierMarkupBid = 1,
                StpMultiplierMarkupAsk = 1,
            };
            
            var assetPairsApiClient = clientGenerator.Generate<IAssetPairsApi>();
            await assetPairsApiClient.List().Dump();
            if (await assetPairsApiClient.Get("t1") == null)
                await assetPairsApiClient.Insert(assetPairContract).Dump();
            var obj = await assetPairsApiClient.Get("t1").Dump();
            //TODO validate values here
            await assetPairsApiClient.Update("t1", new AssetPairUpdateRequest
            {
                Id = "t1",
                MarketId = "m11111"
            }).Dump();
            await assetPairsApiClient.Delete("t1");
        }

        private static async Task CheckAssetsApiWorking(IHttpClientGenerator clientGenerator)
        {
            var asset = new AssetContract
            {
                Id = "t1",
                Name = "1",
                Accuracy = 3,
            };
            
            var assetsApiClient = clientGenerator.Generate<IAssetsApi>();
            await assetsApiClient.List().Dump();
            await assetsApiClient.Insert(asset).Dump();
            await assetsApiClient.Get("t1").Dump();
            asset.Name = "2";
            await assetsApiClient.Update("t1", asset).Dump();
            await assetsApiClient.Delete("t1");
        }

        private static async Task CheckMarketsApiWorking(IHttpClientGenerator clientGenerator)
        {
            var market = new MarketContract
            {
                Id = "m1",
                Name = "1",
            };

            var marketApiClient = clientGenerator.Generate<IMarketsApi>();
            await marketApiClient.List().Dump();
            await marketApiClient.Insert(market).Dump();
            await marketApiClient.Get("m1").Dump();
            market.Name = "1111";
            await marketApiClient.Update("m1", market).Dump();
            await marketApiClient.Delete("m1");
        }

        private static async Task CheckScheduleSettingsApiWorking(IHttpClientGenerator clientGenerator)
        {
            var scheduleSettings = new ScheduleSettingsContract
            {
                Id = "s1",
                Rank = 1000,
                AssetPairRegex = "",
                AssetPairs = new HashSet<string>(){"EURUSD"},
                MarketId = "1",
                IsTradeEnabled = true,
                PendingOrdersCutOff = null,
                Start = new ScheduleConstraintContract{Date = null, DayOfWeek = DayOfWeek.Friday, Time = new TimeSpan(0,0,0)},
                End = new ScheduleConstraintContract{Date = null, DayOfWeek = DayOfWeek.Sunday, Time = new TimeSpan(0,0,0)},
            };

            var scheduleSettingsApiClient = clientGenerator.Generate<IScheduleSettingsApi>();
            await scheduleSettingsApiClient.List().Dump();
            await scheduleSettingsApiClient.Insert(scheduleSettings).Dump();
            await scheduleSettingsApiClient.Get("s1").Dump();
            scheduleSettings.Rank = 100000;
            await scheduleSettingsApiClient.Update("s1", scheduleSettings).Dump();
            await scheduleSettingsApiClient.StateList(new[] {"EURUSD"}).Dump();
            await scheduleSettingsApiClient.Delete("s1");
        }

        private static async Task CheckServiceMaintenanceApiWorking(IHttpClientGenerator clientGenerator)
        {
            var serviceMaintenanceApiClient = clientGenerator.Generate<IServiceMaintenanceApi>();
            await serviceMaintenanceApiClient.Get().Dump();
            await serviceMaintenanceApiClient.Post(true).Dump();
            await serviceMaintenanceApiClient.Post(false).Dump();
        }

        private static async Task CheckTradingConditionsApiWorking(IHttpClientGenerator clientGenerator)
        {
            var tradingCondition = new TradingConditionContract
            {
                Id = "t1",
                Name = "1",
                LegalEntity = "1",
                MarginCall1 = 1,
                MarginCall2 = 1,
                StopOut = 1,
                DepositLimit = 1,
                WithdrawalLimit = 1,
                LimitCurrency = "1",
                BaseAssets = new List<string>(){"1"},
            };

            var tradingConditionApiClient = clientGenerator.Generate<ITradingConditionsApi>();
            await tradingConditionApiClient.List().Dump();
            if (await tradingConditionApiClient.Get("t1") == null)
                await tradingConditionApiClient.Insert(tradingCondition).Dump();
            await tradingConditionApiClient.Get("t1").Dump();
            tradingCondition.Name = "11111";
            await tradingConditionApiClient.Update("t1", tradingCondition).Dump();
        }

        private static async Task CheckTradingInstrumentsApiWorking(IHttpClientGenerator clientGenerator)
        {
            var tradingInstrument = new TradingInstrumentContract
            {
                TradingConditionId = "t1",
                Instrument = "BTCUSD",
                LeverageInit = 1,
                LeverageMaintenance = 1,
                SwapLong = 1,
                SwapShort = 1,
                Delta = 1,
                DealMinLimit = 1,
                DealMaxLimit = 1,
                PositionLimit = 1,
                ShortPosition = true,
                LiquidationThreshold = 1000,
                CommissionRate = 1,
                CommissionMin = 1,
                CommissionMax = 1,
                CommissionCurrency = "1",
            };

            var tradingInstrumentApiClient = clientGenerator.Generate<ITradingInstrumentsApi>();
            await tradingInstrumentApiClient.List(null).Dump();
            await tradingInstrumentApiClient.Insert(tradingInstrument).Dump();
            tradingInstrument.LeverageInit = 2;
            await tradingInstrumentApiClient.Update("t1", "BTCUSD", tradingInstrument).Dump();
            await tradingInstrumentApiClient.AssignCollection("t1", new[] {"EURUSD", "EURCHF", "BTCUSD"}).Dump();
            foreach (var tradingInstrumentContract in await tradingInstrumentApiClient.List(null))
            {
                await tradingInstrumentApiClient.Delete(tradingInstrumentContract.TradingConditionId,
                    tradingInstrumentContract.Instrument);
            }
        }

        private static async Task CheckTradingRoutesApiWorking(IHttpClientGenerator clientGenerator)
        {
            var tradingRoute = new MatchingEngineRouteContract
            {
                Id = "t1",
                Rank = 100,
                TradingConditionId = "t1",
                ClientId = "1",
                Instrument = "BTCUSD",
                Type = OrderDirectionContract.Buy,
                MatchingEngineId = "m1",
                Asset = "BTC",
                RiskSystemLimitType = "11",
                RiskSystemMetricType = "11",
            };

            var tradingRouteApiClient = clientGenerator.Generate<ITradingRoutesApi>();
            await tradingRouteApiClient.List().Dump();
            await tradingRouteApiClient.Insert(tradingRoute).Dump();
            tradingRoute.Rank = 10000;
            await tradingRouteApiClient.Update("t1", tradingRoute).Dump();
            await tradingRouteApiClient.Delete("t1");
        }

        [CanBeNull]
        public static T Dump<T>(this T o)
        {
            var str = o is string s ? s : JsonConvert.SerializeObject(o);
            Console.WriteLine("{0}. {1}", ++_counter, str);
            return o;
        }

        [ItemCanBeNull]
        public static async Task<T> Dump<T>(this Task<T> t)
        {
            var obj = await t;
            obj.Dump();
            return obj;
        }

        public static async Task Dump(this Task o)
        {
            await o;
            "ok".Dump();
        }
    }
}