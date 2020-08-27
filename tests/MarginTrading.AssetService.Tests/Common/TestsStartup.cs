using Autofac;
using JetBrains.Annotations;
using Lykke.SettingsReader;
using MarginTrading.AssetService.Modules;
using MarginTrading.AssetService.Tests.AutofacModules;
using Microsoft.Extensions.Hosting;

namespace MarginTrading.AssetService.Tests.Common
{
    public class TestsStartup : Startup
    {
        public TestsStartup(IHostEnvironment env) : base(env)
        {
        }

        [UsedImplicitly]
        public override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ServiceModule(_mtSettingsManager.Nested(x => x.MarginTradingAssetService), Log));
            builder.RegisterModule(new TestMsSqlModule());
        }
    }
}