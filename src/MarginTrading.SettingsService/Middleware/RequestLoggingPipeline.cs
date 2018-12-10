using Microsoft.AspNetCore.Builder;

namespace MarginTrading.SettingsService.Middleware
{
    public class RequestLoggingPipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<RequestsLoggingMiddleware>();
        }
    }
}
