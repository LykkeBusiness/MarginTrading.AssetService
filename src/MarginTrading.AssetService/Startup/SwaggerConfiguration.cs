using Microsoft.AspNetCore.Builder;

namespace MarginTrading.AssetService.Startup
{
    public static class SwaggerConfiguration
    {
        public static IApplicationBuilder ConfigureSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(a => a.SwaggerEndpoint("/swagger/v1/swagger.json", "Nova 2 Assets API"));

            return app;
        }
    }
}