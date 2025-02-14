// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using MarginTrading.AssetService.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MarginTrading.AssetService
{
    internal sealed class Program
    {
        public static string ApplicationName => "AssetService";

        public static async Task Main(string[] args)
        {
            if (EF.IsDesignTime) {
                await new HostBuilder().Build().RunAsync();
                return;
            }
            
            await new StartupWrapper().StartAsync(async () =>
            {
                var builder = WebApplication.CreateBuilder(args);

                var (configuration, settingsManager) = builder.BuildConfiguration();

                builder.Services.RegisterInfrastructureServices(
                    settingsManager.CurrentValue.MarginTradingAssetServiceClient);

                builder.ConfigureHost(configuration, settingsManager);
                
                var app = builder.Build();

                await app
                    .Configure()
                    .RunAsync();
            });
        }
    }
}
