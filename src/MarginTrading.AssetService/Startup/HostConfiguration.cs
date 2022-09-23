using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.SettingsReader;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Correlation.Serilog;
using MarginTrading.AssetService.Modules;
using MarginTrading.AssetService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MarginTrading.AssetService.Startup
{
    public static class HostConfiguration
    {
        public static IHostBuilder ConfigureHost(this WebApplicationBuilder builder, IConfiguration configuration, IReloadingManager<AppSettings> settingsManager)
        {
            return builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((ctx, cBuilder) =>
                {
                    var assetSettingsManager = settingsManager.Nested(x => x.MarginTradingAssetService);
                    
                    cBuilder.RegisterModule(new ServiceModule(assetSettingsManager));
                    if (!ctx.HostingEnvironment.IsEnvironment("test"))
                    {
                        cBuilder.RegisterModule(new ClientsModule(settingsManager));
                        cBuilder.RegisterModule(new MsSqlModule(assetSettingsManager));
                        cBuilder.RegisterModule(new CqrsModule(assetSettingsManager.CurrentValue.Cqrs, assetSettingsManager.CurrentValue.InstanceId));
                        cBuilder.RegisterModule(new RabbitMqModule(assetSettingsManager));
                    }
                })
                .UseSerilog((_, cfg) =>
                {
                    var assembly = typeof(Program).Assembly;
                    var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;
                    var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
                    
                    cfg.ReadFrom.Configuration(configuration)
                        .Enrich.WithProperty("Application", title)
                        .Enrich.WithProperty("Version", version)
                        .Enrich.WithProperty("Environment", environment)
                        .Enrich.WithProperty("BrokerId", settingsManager.CurrentValue.MarginTradingAssetService.BrokerId)
                        .Enrich.With(new CorrelationLogEventEnricher("CorrelationId", new CorrelationContextAccessor()));
                });
        }
    }
}