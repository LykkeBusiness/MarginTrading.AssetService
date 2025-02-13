using Kathe.LogEnrichment;
using Kathe.LogEnrichment.Outbound;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;

namespace Kathe.Configuration;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddSerilogJson(
        this IConfigurationBuilder builder,
        IHostEnvironment env)
    {
        return builder.AddJsonFile(Path.Combine(env.ContentRootPath, "appsettings.Serilog.json"));
    }
    
    public static IServiceCollection AddEnrichedLogging(this IServiceCollection services)
    {
        services.LogOutboundRequests();
        services.AddHttpContextAccessor();
        return services;
    }
    
    public static IServiceCollection LogOutboundRequests(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor
            .Singleton<IHttpMessageHandlerBuilderFilter, OutboundRequestLoggingHandlerBuilderFilter>());
        return services;
    }

    public static IApplicationBuilder UseEnrichedLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<LogEnrichmentMiddleware>();
        return app;
    }
}