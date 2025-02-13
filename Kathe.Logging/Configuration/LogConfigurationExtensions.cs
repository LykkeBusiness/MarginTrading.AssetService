using System.Reflection;

using Kathe.LogEnrichment.Correlation;
using Kathe.LogEnrichment.Correlation.Serilog;
using Kathe.LogEnrichment.Exceptions;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;

namespace Kathe.Configuration;

public static class LogConfigurationExtensions
{
    public static class Conventions
    {
        public static class Configuration
        {
            public static readonly string BrokerId = "BrokerId";

        }
        public static class Environment
        {
            public static readonly string SeqUrl = "SEQ_URL";
            public static readonly string SeqApiKey = "SEQ_API_KEY";
            public static readonly string AspnetCore = "ASPNETCORE_ENVIRONMENT";
            public static readonly string Nova = "NOVA_ENVIRONMENT";
        }
    }

    /// <summary>
    /// Adds default configuration and makes overrides using IConfiguration
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <param name="config"></param>
    /// <param name="applicationName"></param>
    /// <param name="additionalLogProperties"></param>
    /// <returns></returns>
    public static LoggerConfiguration AddDefaultConfig(this LoggerConfiguration loggerConfiguration, IConfiguration config, string applicationName, params (string Key, string Value)[] additionalLogProperties)
    {
        loggerConfiguration.AddDefaultConfig(applicationName, additionalLogProperties);

        var brokerId = config.GetChildren().FirstOrDefault(x => x.Key == Conventions.Configuration.BrokerId)?.Value;
        if (brokerId != null)
            loggerConfiguration.Enrich.WithProperty(Conventions.Configuration.BrokerId, brokerId);
        
        loggerConfiguration = loggerConfiguration.ReadFrom.Configuration(config);
        
        return loggerConfiguration;
    }

    /// <summary>
    /// Adds default configuration when no additional IConfiguration provided
    /// </summary>
    /// <param name="loggerConfiguration"></param>
    /// <param name="applicationName"></param>
    /// <param name="additionalLogProperties"></param>
    /// <returns></returns>
    public static LoggerConfiguration AddDefaultConfig(this LoggerConfiguration loggerConfiguration, string applicationName, params (string Key, string Value)[] additionalLogProperties)
    {
        loggerConfiguration = loggerConfiguration
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        
        foreach (var kvp in additionalLogProperties)
            loggerConfiguration = loggerConfiguration.Enrich.WithProperty(kvp.Key, kvp.Value);
        
        loggerConfiguration = loggerConfiguration
            .Enrich.WithProperty(LogConfiguration.ApplicationNameProperty, applicationName)
            .Enrich.With<ExceptionFormattingEnricher>()
            .Enrich.With(new CorrelationLogEventEnricher(new CorrelationContextAccessor()))
            .Enrich.FromLogContext();

        loggerConfiguration = EnrichWithAssemblyInformation(loggerConfiguration);

        loggerConfiguration = EnrichWithEnvironmentInformation(loggerConfiguration);

        loggerConfiguration = AddConsole(loggerConfiguration);
        
        loggerConfiguration = AddFile(loggerConfiguration, applicationName);
            
        loggerConfiguration = AddSeq(loggerConfiguration);

        return loggerConfiguration;
    }

    private static LoggerConfiguration EnrichWithEnvironmentInformation(LoggerConfiguration loggerConfiguration)
    {
        var novaEnvironment = Environment.GetEnvironmentVariable(Conventions.Environment.Nova);
        if (!string.IsNullOrEmpty(novaEnvironment))
            loggerConfiguration = loggerConfiguration.Enrich.WithProperty("NovaEnvironment", novaEnvironment);
        
        var environment = Environment.GetEnvironmentVariable(Conventions.Environment.AspnetCore);
        if (!string.IsNullOrEmpty(novaEnvironment))
            loggerConfiguration = loggerConfiguration.Enrich.WithProperty("Environment", environment);

        return loggerConfiguration;
    }

    private static LoggerConfiguration EnrichWithAssemblyInformation(LoggerConfiguration loggerConfiguration)
    {
        var a = Assembly.GetCallingAssembly();
        var title = a.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;
        var version = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;

        loggerConfiguration = loggerConfiguration
            .Enrich.WithProperty("Application", title)
            .Enrich.WithProperty("Version", version);
        
        return loggerConfiguration;
    }

    private static LoggerConfiguration AddConsole(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration = loggerConfiguration
            .WriteTo.Async(
                c => c.Console(outputTemplate: "[{Timestamp:u}] [{Level:u3}] [{Application}:{Version}:{Environment}] [{BrokerId}] [{CorrelationId}] - {info} {Message:lj} {NewLine}{Exception}")
            );
        return loggerConfiguration;
    }

    private static LoggerConfiguration AddFile(LoggerConfiguration loggerConfiguration, string applicationName)
    {
        loggerConfiguration = loggerConfiguration
            .WriteTo.Async(
                c => c
                    .File(
                        path: $"logs/MTCore/{applicationName}.log",
                        outputTemplate: "[{Timestamp:u}] [{Level:u3}] [{Application}:{Version}:{Environment}] [{BrokerId}] [{CorrelationId}] - {info} {Message:lj} {NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day
                    )
            );
        return loggerConfiguration;
    }

    private static LoggerConfiguration AddSeq(LoggerConfiguration loggerConfiguration)
    {
        var seqUrl = Environment.GetEnvironmentVariable(Conventions.Environment.SeqUrl);
        if (!string.IsNullOrEmpty(seqUrl))
        {
            loggerConfiguration = loggerConfiguration.WriteTo.Seq(
                serverUrl: seqUrl,
                apiKey: Environment.GetEnvironmentVariable(Conventions.Environment.SeqApiKey)
            );
        }

        return loggerConfiguration;
    }
}