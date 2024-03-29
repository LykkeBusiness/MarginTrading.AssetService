﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.AssetService.Services.Mapping;
using MarginTrading.AssetService.StorageInterfaces.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using SqlRepos = MarginTrading.AssetService.SqlRepositories.Repositories;

namespace MarginTrading.AssetService.Migration
{
    internal static class Program
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

            var tradingRoutesRepos = new ITradingRoutesRepository[]
            {
                new SqlRepos.TradingRoutesRepository(convertService, sqlConnStr, new NullLogger<SqlRepos.TradingRoutesRepository>())
            };

            if (option == "2")
            {
                tradingRoutesRepos = tradingRoutesRepos.Reverse().ToArray();
            }

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