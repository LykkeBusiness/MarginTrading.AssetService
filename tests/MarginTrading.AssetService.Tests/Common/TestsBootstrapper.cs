using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Lykke.Common.MsSql;
using Lykke.Snow.Mdm.Contracts.Api;
using Lykke.Snow.Mdm.Contracts.Models.Contracts;
using Lykke.Snow.Mdm.Contracts.Models.Responses;
using MarginTrading.AssetService.Controllers;
using MarginTrading.AssetService.Modules;
using MarginTrading.AssetService.SqlRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
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
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer(registerDependenciesAction)
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<TestsStartup>();
                    webHost.ConfigureServices(x =>
                    {
                        var assembly = typeof(ClientProfilesController).Assembly;
                        x.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
                    });
                });

            var host = await hostBuilder.StartAsync();

            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Add("api-key", "margintrading");

            return client;
        }
    }
}