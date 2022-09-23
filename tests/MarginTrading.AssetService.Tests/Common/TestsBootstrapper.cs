using System;
using System.Net.Http;
using Lykke.Common.MsSql;
using Lykke.Cqrs;
using Lykke.Snow.Mdm.Contracts.Api;
using MarginTrading.AssetService.SqlRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace MarginTrading.AssetService.Tests.Common
{
    public static class TestBootstrapper
    {
        private const string EnvironmentName = "test";
        private const string AppSettingsFileName = "appsettings.test.json";
        
        public static HttpClient CreateTestClientWithInMemoryDb(Action<IServiceCollection> mockDependencies = null)
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder
                        .UseSetting("SettingsUrl", AppSettingsFileName)
                        .UseEnvironment(EnvironmentName)
                        .ConfigureServices(services =>
                        {
                            // register mocked cqrs engine
                            services.AddSingleton(new Mock<ICqrsEngine>().Object);

                            // register in-memory database
                            var options = new DbContextOptionsBuilder<AssetDbContext>()
                                .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid().ToString()}")
                                .Options;
                            var contextFactory =
                                new MsSqlContextFactory<AssetDbContext>(_ => new AssetDbContext(options), options);
                            services.AddSingleton(contextFactory);
                            services.AddSingleton<Lykke.Common.MsSql.IDbContextFactory<AssetDbContext>>(contextFactory);

                            // mocking remote services with defaults
                            services.AddSingleton(new Mock<IUnderlyingsApi>().Object);
                            services.AddSingleton(new Mock<IRegulatoryTypesApi>().Object);
                            services.AddSingleton(new Mock<IRegulationsApi>().Object);
                            services.AddSingleton(new Mock<IRegulatoryProfilesApi>().Object);
                            services.AddSingleton(new Mock<IRegulatorySettingsApi>().Object);
                            services.AddSingleton(new Mock<IBrokerSettingsApi>().Object);
                            services.AddSingleton(new Mock<IUnderlyingCategoriesApi>().Object);

                            mockDependencies?.Invoke(services);
                        });
                });
            
            var client = application.CreateDefaultClient();
            client.DefaultRequestHeaders.Add("api-key", "margintrading");

            return client;
        }
    }
}