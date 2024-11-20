using System;
using System.IO;

using Lykke.HttpClientGenerator.Infrastructure;
using Lykke.SettingsReader.SettingsTemplate;
using Lykke.Snow.Common.AssemblyLogging;
using Lykke.Snow.Common.Correlation;
using Lykke.Snow.Common.Correlation.Cqrs;
using Lykke.Snow.Common.Correlation.Http;
using Lykke.Snow.Common.Correlation.RabbitMq;
using Lykke.Snow.Common.Startup;
using Lykke.Snow.Common.Startup.ApiKey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MarginTrading.AssetService.Startup
{
    public static class DependencyInjectionSetup
    {
        private static readonly string ApiName = "Nova 2 Assets API";

        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
            ClientSettings assetServiceClientConfiguration)
        {
            services.AddAssemblyLogger();
            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddMemoryCache(options => options.ExpirationScanFrequency = TimeSpan.FromMinutes(1));
            services.AddApiKeyAuth(assetServiceClientConfiguration);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = $"{ApiName} API"
                    });

                var contractsXmlPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "MarginTrading.AssetService.Contracts.xml");
                options.IncludeXmlComments(contractsXmlPath);
                // to avoid naming collision with Cronut packages
                options.CustomSchemaIds(i => i.FullName);
                if (!string.IsNullOrWhiteSpace(assetServiceClientConfiguration?.ApiKey))
                {
                    options.AddApiKeyAwareness();
                }
            }).AddSwaggerGenNewtonsoftSupport();

            services.AddSingleton<CorrelationContextAccessor>();
            services.AddSingleton<RabbitMqCorrelationManager>();
            services.AddSingleton<CqrsCorrelationManager>();
            services.AddTransient<HttpCorrelationHandler>();

            services.AddApplicationInsightsTelemetry();

            services.AddTransient<IValidationApiExceptionHandler, DefaultValidationApiExceptionHandler>();

            services.AddSettingsTemplateGenerator();

            return services;
        }
    }
}