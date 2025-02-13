using System.Reflection;

using Kathe;
using Kathe.Configuration;

using Lykke.SettingsReader;
using Lykke.SettingsReader.ConfigurationProvider;

using MarginTrading.AssetService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace MarginTrading.AssetService.Startup
{
    public static class ConfigurationBuilder
    {
        public static (IConfigurationRoot, IReloadingManager<AppSettings>) BuildConfiguration(this WebApplicationBuilder builder)
        {
            builder.Environment.ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configurationBuilder = builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddSerilogJson(builder.Environment)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            if (Environment.GetEnvironmentVariable("SettingsUrl")?.StartsWith("http") ?? false)
            {
                configurationBuilder.AddHttpSourceConfiguration();
            }

            var configuration = configurationBuilder.Build();

            var settingsManager = configuration.LoadSettings<AppSettings>(_ => { });

            return (configuration, settingsManager);
        }
    }
}