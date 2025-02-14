using System;
using System.Reflection;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Kathe;
using Kathe.Configuration;

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
            IReloadingManager<AppSettings> settings)
        {
            return builder.Host
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(
                    (ctx, cBuilder) =>
                    {
                        var assetSettings =
                            settings.Nested(x => x.MarginTradingAssetService);

                        // todo: what is this for? @andrey @misha
                        var isTestEnv = ctx.HostingEnvironment.IsEnvironment("test");
                        settings.CurrentValue.ValidateSettings(checkDependencies: !isTestEnv).GetAwaiter().GetResult();

                        cBuilder.RegisterModule(new ServiceModule(assetSettings));
                        if (!isTestEnv)
                        {
                            cBuilder.RegisterModule(new ClientsModule(settings));
                            cBuilder.RegisterModule(new MsSqlModule(assetSettings));
                            cBuilder.RegisterModule(
                                new CqrsModule(
                                    assetSettings.CurrentValue.Cqrs,
                                    assetSettings.CurrentValue.InstanceId));
                            cBuilder.RegisterModule(new RabbitMqModule(assetSettings));
                        }
                    })
                .UseSerilog(LogConfiguration.BuildSerilogLogger(configuration, Program.ApplicationName));
        }
    }
}