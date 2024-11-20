using System;

using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.HttpClientGenerator;
using Lykke.SettingsReader.SettingsTemplate;
using Lykke.Snow.Common.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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
            app.MapSettingsTemplate();

            return app;
        }
    }
}