using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MarginTrading.AssetService.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace MarginTrading.AssetService.Tests.Common
{
    public class TestBootstrapper
    {
        public static async Task<HttpClient> CreateTestClientWithInMemoryDb(Action<ContainerBuilder> registerDependenciesAction = null)
        {
            System.Environment.SetEnvironmentVariable("SettingsUrl", "appsettings.test.json");

            var hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
                
            hostBuilder = registerDependenciesAction == null ? hostBuilder : hostBuilder.ConfigureContainer(registerDependenciesAction);
            hostBuilder = hostBuilder.ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<TestsStartup>();
                    webHost.ConfigureServices(x =>
                    {
                        var assembly = typeof(ClientProfilesController).Assembly;
                        x.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
                        x.AddSingleton<ILogger, NullLogger>();
                    });
                    webHost.UseSentry(o =>
                    {
                        o.Dsn = "https://7ec2d3a753d645d3858a6d2640030196@o1091203.ingest.sentry.io/6107924";
                        o.Debug = true;
                        o.TracesSampleRate = 1.0;
                    });
                });

            var host = await hostBuilder.StartAsync();

            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Add("api-key", "margintrading");

            return client;
        }
    }
}