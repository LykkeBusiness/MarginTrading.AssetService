using System.IO;
using System.Reflection;
using Lykke.Logs.Serilog;
using Lykke.SettingsReader;
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
                
            var configuration = builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddSerilogJson(builder.Environment)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .Build();
        
            var settingsManager = configuration.LoadSettings<AppSettings>(_ => { });

            return (configuration, settingsManager);
        }
    }
}