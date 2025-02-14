// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Kathe.LogEnrichment.Correlation.Cqrs;
using Kathe.LogEnrichment.Correlation.Http;
using Kathe.LogEnrichment.Correlation.RabbitMq;
using Kathe.LogEnrichment.Correlation.Serilog;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ISerilogLogger = Serilog.ILogger;

namespace Kathe.LogEnrichment.Correlation
{
    public static class CorrelationExtensions
    {
        /// <summary>
        /// Adds correlation services to the application.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorrelation(this IServiceCollection services)
        {
            services.AddSingleton<CorrelationContextAccessor>();
            services.AddSingleton<RabbitMqCorrelationManager>();
            services.AddSingleton<CqrsCorrelationManager>();
            services.AddTransient<HttpCorrelationHandler>();
            return services;
        }
        
        public static ISerilogLogger WithCorrelationLogEventEnricher(
            this ISerilogLogger logger,
            string propertyName,
            CorrelationContextAccessor correlationContextAccessor)
        {
            return logger.ForContext(new CorrelationLogEventEnricher(correlationContextAccessor, propertyName));
        }
        
        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpCorrelationMiddleware>();
            return app;
        }

        /// <summary>
        /// Generates a correlation identifier with prefix and memorises it for future use.
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="prefix">The prefix to be used in correlation identifier</param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetCorrelationWithPrefixIncludingLogging(
            this CorrelationContextAccessor accessor,
            string prefix, 
            ILogger logger)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException(nameof(prefix));
            
            var correlationId = $"{prefix}-{Guid.NewGuid():N}";
            accessor.CorrelationContext = new CorrelationContext(correlationId);
            logger.LogDebug("Correlation context with id '{CorrelationId}' was created", correlationId);
        }
        
        /// <summary>
        /// Provides correlation identifier if it has already been generated before, otherwise generates new one
        /// and memorises it for future use.
        /// </summary>
        /// <param name="accessor"></param>
        /// <returns>The correlation identifier</returns>
        public static string GetOrGenerateCorrelationId(this CorrelationContextAccessor accessor)
        {
            var correlationId = accessor.CorrelationContext?.CorrelationId;
            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString("N");
                accessor.CorrelationContext = new CorrelationContext(correlationId);
            }

            return correlationId;
        }
    }
}