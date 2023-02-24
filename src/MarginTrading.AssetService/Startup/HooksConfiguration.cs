using System;
using MarginTrading.AssetService.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Startup
{
    public static class HooksConfiguration
    {
        public static void RegisterHooks(this WebApplication app)
        {
            app.Lifetime.ApplicationStarted.Register(() => OnApplicationStarted(app));
            app.Lifetime.ApplicationStopping.Register(() => OnApplicationStopping(app));
        }
    
        private static void OnApplicationStarted(WebApplication app)
        {
            var log = app.Services.GetService<ILogger<Program>>();

            try
            {
                app.Services.GetRequiredService<IStartupManager>().Start();

                log?.LogInformation("Nova 2 Assets API started");
            }
            catch (Exception ex)
            {
                log?.LogCritical(ex, "Error on startup");
                app.StopAsync().GetAwaiter().GetResult();
            }
        }

        private static void OnApplicationStopping(WebApplication app)
        {
            var log = app.Services.GetService<ILogger<Program>>();

            try
            {
                app.Services.GetRequiredService<IShutdownManager>().Stop();
            }
            catch (Exception ex)
            {
                log?.LogError(ex, "Error on shutdown");
                throw;
            }
        }
    }
}