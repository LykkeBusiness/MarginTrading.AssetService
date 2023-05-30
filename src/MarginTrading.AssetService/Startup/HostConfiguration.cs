using System;
using System.Reflection;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Lykke.SettingsReader;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Correlation.Serilog;
using Lykke.Snow.Common.Startup;

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
        public static IHostBuilder ConfigureHost(
            this WebApplicationBuilder builder,
            IConfiguration configuration,
            IReloadingManager<AppSettings> settings) =>
            builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>((ctx, cBuilder) =>
                {
                    var assetSettings =
                        settings.Nested(x => x.MarginTradingAssetService);

                    var isTestEnv = ctx.HostingEnvironment.IsEnvironment("test");
                    settings.CurrentValue.ValidateSettings(checkDependencies: !isTestEnv);

                    cBuilder.RegisterModule(new ServiceModule(assetSettings));
                    if (!isTestEnv)
                    {
                        cBuilder.RegisterModule(new ClientsModule(settings));
                        cBuilder.RegisterModule(new MsSqlModule(assetSettings));
                        cBuilder.RegisterModule(new CqrsModule(
                            assetSettings.CurrentValue.Cqrs,
                            assetSettings.CurrentValue.InstanceId));
                        cBuilder.RegisterModule(new RabbitMqModule(assetSettings));
                    }
                })
                .UseSerilog((_, cfg) =>
                {
                    var a = typeof(Program).Assembly;
                    var title =
                        a.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
                        string.Empty;
                    var version =
                        a.GetCustomAttribute<
                                AssemblyInformationalVersionAttribute>()
                            ?.InformationalVersion ?? string.Empty;
                    var environment =
                        Environment.GetEnvironmentVariable(
                            "ASPNETCORE_ENVIRONMENT") ?? string.Empty;

                    cfg.ReadFrom.Configuration(configuration)
                        .Enrich.WithProperty("Application", title)
                        .Enrich.WithProperty("Version", version)
                        .Enrich.WithProperty("Environment", environment)
                        .Enrich.WithProperty("BrokerId",
                            settings.CurrentValue.MarginTradingAssetService
                                .BrokerId)
                        .Enrich.With(new CorrelationLogEventEnricher(
                            "CorrelationId", new CorrelationContextAccessor()));
                });
    }
}