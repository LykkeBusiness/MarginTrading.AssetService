﻿// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MarginTrading.SettingsService.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace MarginTrading.SettingsService
{
    internal sealed class Program
    {
        public static string EnvInfo => Environment.GetEnvironmentVariable("ENV_INFO");

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"{PlatformServices.Default.Application.ApplicationName} version {PlatformServices.Default.Application.ApplicationVersion}");
            
            var restartAttemptsLeft = int.TryParse(Environment.GetEnvironmentVariable("RESTART_ATTEMPTS_NUMBER"),
                out var restartAttemptsFromEnv) 
                ? restartAttemptsFromEnv
                : int.MaxValue;
            var restartAttemptsInterval = int.TryParse(Environment.GetEnvironmentVariable("RESTART_ATTEMPTS_INTERVAL_MS"),
                out var restartAttemptsIntervalFromEnv) 
                ? restartAttemptsIntervalFromEnv
                : 10000;

            while (restartAttemptsLeft > 0)
            {
                try
                {
                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddUserSecrets<Startup>()
                        .AddEnvironmentVariables()
                        .Build();
                    
                    var host = WebHost.CreateDefaultBuilder()
                        .UseConfiguration(configuration)
                        .UseStartup<Startup>()
                        .UseApplicationInsights()
                        .Build();

                    await host.RunAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}{Environment.NewLine}{e.StackTrace}{Environment.NewLine}Restarting...");
                    LogLocator.CommonLog?.WriteFatalErrorAsync(
                        "MT SettingsService", "Restart host", $"Attempts left: {restartAttemptsLeft}", e);
                    restartAttemptsLeft--;
                    Thread.Sleep(restartAttemptsInterval);
                }
            }

            Console.WriteLine("Terminated");
        }
    }
}
