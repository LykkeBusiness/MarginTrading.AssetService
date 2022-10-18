using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarginTrading.AssetService.Tests.Common
{
    internal static class DatabaseHelper
    {
        public static DbContextOptions<T> CreateNewContextOptions<T>(string databaseName) where T : DbContext
        {
            // https://stackoverflow.com/questions/38890269/how-to-isolate-ef-inmemory-database-per-xunit-test
            // Typically, EF creates a single IServiceProvider for all contexts
            // of a given type in an AppDomain - meaning all context instances
            // share the same InMemory database instance. By allowing one to be
            // passed in, you can control the scope of the InMemory database.

            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseInMemoryDatabase(databaseName: databaseName)
                .UseInternalServiceProvider(serviceProvider)
                .EnableSensitiveDataLogging();

            return builder.Options;
        }
    }
}