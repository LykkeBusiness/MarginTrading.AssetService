using System;

using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.HttpClientGenerator;
using Lykke.Snow.Common.AssemblyLogging;
using Lykke.Snow.Common.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarginTrading.AssetService.Startup
{
    public static class ApplicationConfiguration
    {
        public static WebApplication Configure(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCorrelation();
#if DEBUG
            app.UseLykkeMiddleware(
                Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName,
                ex => ex.ToString(), false);
#else
            app.UseLykkeMiddleware("Asset Service", ex => new ErrorResponse {ErrorMessage = ex.Message}, false);
#endif
            app.UseRefitExceptionHandler();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.ConfigureSwagger();
            app.MapControllers();
            app.RegisterHooks();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                try
                {
                    app.Services.GetRequiredService<AssemblyLogger>()
                        .StartLogging();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to start");
                    app.Lifetime.StopApplication();
                    return;
                }
                logger.LogInformation($"{nameof(Startup)} started");
            });
            
            return app;
        }
    }
}