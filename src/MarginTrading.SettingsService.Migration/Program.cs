using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.SettingsReader.ReloadingManager;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Services;
using MarginTrading.SettingsService.StorageInterfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using SqlRepos = MarginTrading.SettingsService.SqlRepositories.Repositories;
using AzureRepos = MarginTrading.SettingsService.AzureRepositories.Repositories;

namespace MarginTrading.SettingsService.Migration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Settings migration service.");

            var sqlConnStr = Environment.GetEnvironmentVariable("SqlConnectionString");
            var azureConnStr = Environment.GetEnvironmentVariable("AzureConnString");
            if (string.IsNullOrWhiteSpace(sqlConnStr) || string.IsNullOrWhiteSpace(azureConnStr))
            {
                Console.WriteLine("First set env vars: SqlConnectionString, AzureConnString. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Data in the destination will remain, errors will be skipped.");
            Console.WriteLine("Please select option:");
            Console.WriteLine("1. From Azure to SQL (1)");
            Console.WriteLine("2. From SQL to Azure (2)");
            Console.WriteLine(">>>");

            var option = "";
            while (true)
            {
                option = Console.ReadLine()?.Trim();
                if (option == "1" || option == "2")
                    break;
                Console.WriteLine("Wrong choice, try again");
            }

            var convertService = new ConvertService();
            var azureRm = ConstantReloadingManager.From(azureConnStr);
            var fakeLogger = new AggregateLogger();

            var assetsRepos = new IAssetsRepository[]
            {
                new AzureRepos.AssetsRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.AssetsRepository(convertService, sqlConnStr, fakeLogger),
            };
            var assetPairsRepos = new IAssetPairsRepository[]
            {
                new AzureRepos.AssetPairsRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.AssetPairsRepository(convertService, sqlConnStr, fakeLogger),
            };
            var marketsRepos = new IMarketRepository[]
            {
                new AzureRepos.MarketRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.MarketRepository(convertService, sqlConnStr, fakeLogger),
            };
            var scheduleSettingsRepos = new IScheduleSettingsRepository[]
            {
                new AzureRepos.ScheduleSettingsRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.ScheduleSettingsRepository(convertService, sqlConnStr, fakeLogger),
            };
            var tradingConditionsRepos = new ITradingConditionsRepository[]
            {
                new AzureRepos.TradingConditionsRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.TradingConditionsRepository(convertService, sqlConnStr, fakeLogger),
            };
            var tradingInstrumentsRepos = new ITradingInstrumentsRepository[]
            {
                new AzureRepos.TradingInstrumentsRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.TradingInstrumentsRepository(convertService, sqlConnStr, fakeLogger),
            };
            var tradingRoutesRepos = new ITradingRoutesRepository[]
            {
                new AzureRepos.TradingRoutesRepository(fakeLogger, convertService, azureRm), 
                new SqlRepos.TradingRoutesRepository(convertService, sqlConnStr, fakeLogger),
            };

            if (option == "2")
            {
                assetsRepos = assetsRepos.Reverse().ToArray();
                assetPairsRepos = assetPairsRepos.Reverse().ToArray();
                marketsRepos = marketsRepos.Reverse().ToArray();
                scheduleSettingsRepos = scheduleSettingsRepos.Reverse().ToArray();
                tradingConditionsRepos = tradingConditionsRepos.Reverse().ToArray();
                tradingInstrumentsRepos = tradingInstrumentsRepos.Reverse().ToArray();
                tradingRoutesRepos = tradingRoutesRepos.Reverse().ToArray();
            }

            var assets = await assetsRepos.First().GetAsync();
            var assetsSucceeded = 0;
            foreach (var asset in assets)
            {
                if (await assetsRepos.Last().TryInsertAsync(asset))
                    assetsSucceeded++;
            }
            Console.WriteLine($"Assets succeeded: {assetsSucceeded}, failed: {assets.Count - assetsSucceeded}.");

            var assetPairs = await assetPairsRepos.First().GetAsync();
            var assetPairsSucceeded = 0;
            foreach (var assetPair in assetPairs)
            {
                if (await assetPairsRepos.Last().TryInsertAsync(assetPair))
                    assetPairsSucceeded++;
            }
            Console.WriteLine($"Asset pairs succeeded: {assetPairsSucceeded}, failed: {assetPairs.Count - assetPairsSucceeded}.");

            var markets = await marketsRepos.First().GetAsync();
            var marketsSucceeded = 0;
            foreach (var market in markets)
            {
                if (await marketsRepos.Last().TryInsertAsync(market))
                    marketsSucceeded++;
            }
            Console.WriteLine($"Markets succeeded: {marketsSucceeded}, failed: {markets.Count - marketsSucceeded}.");

            var scheduleSettings = await scheduleSettingsRepos.First().GetAsync();
            var scheduleSettingsSucceeded = 0;
            foreach (var scheduleSetting in scheduleSettings)
            {
                if (await scheduleSettingsRepos.Last().TryInsertAsync(scheduleSetting))
                    scheduleSettingsSucceeded++;
            }
            Console.WriteLine($"Schedule settings succeeded: {scheduleSettingsSucceeded}, failed: {scheduleSettings.Count - scheduleSettingsSucceeded}.");

            var tradingConditions = await tradingConditionsRepos.First().GetAsync();
            var tradingConditionsSucceeded = 0;
            foreach (var tradingCondition in tradingConditions)
            {
                if (await tradingConditionsRepos.Last().TryInsertAsync(tradingCondition))
                    tradingConditionsSucceeded++;
            }
            Console.WriteLine($"Trading conditions succeeded: {tradingConditionsSucceeded}, failed: {tradingConditions.Count - tradingConditionsSucceeded}.");

            var tradingInstruments = await tradingInstrumentsRepos.First().GetAsync();
            var tradingInstrumentsSucceeded = 0;
            foreach (var tradingInstrument in tradingInstruments)
            {
                if (await tradingInstrumentsRepos.Last().TryInsertAsync(tradingInstrument))
                    tradingInstrumentsSucceeded++;
            }
            Console.WriteLine($"Trading instruments succeeded: {tradingInstrumentsSucceeded}, failed: {tradingInstruments.Count - tradingInstrumentsSucceeded}.");

            var tradingRoutes = await tradingRoutesRepos.First().GetAsync();
            var tradingRoutesSucceeded = 0;
            foreach (var tradingRoute in tradingRoutes)
            {
                if (await tradingRoutesRepos.Last().TryInsertAsync(tradingRoute))
                    tradingRoutesSucceeded++;
            }
            Console.WriteLine($"Trading routes succeeded: {tradingRoutesSucceeded}, failed: {tradingRoutes.Count - tradingRoutesSucceeded}.");

            Console.WriteLine("Finished! Press any key to exit.");
            Console.ReadKey();
        }
    }
}