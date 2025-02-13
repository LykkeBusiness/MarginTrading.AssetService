using Microsoft.Extensions.Configuration;

using Serilog;

namespace Kathe.Configuration;

public static class LogConfiguration
{
    public static readonly string ApplicationNameProperty = "Application";

    /// <summary>
    /// Creates a <see cref="Serilog.ILogger"/> that you can use. Note, it does not set up a global logger but
    /// this is what we're trying to avoid. 
    ///     * using structured json formatter
    ///     * Enriched from log context
    /// </summary>
    /// <param name="config">the configuration to use to add additional configuration settings to.</param>
    /// <param name="applicationName">
    ///     The name of the application to add to log context. Useful when the application / service
    ///     consists of multiple services.
    /// </param>
    /// <param name="additionalLogProperties">Additional properties to add to diagnostic context</param>
    public static ILogger BuildSerilogLogger(IConfiguration config, string applicationName,
        params (string Key, string Value)[] additionalLogProperties)
    {
        var logger = new LoggerConfiguration()
            .AddDefaultConfig(config, applicationName, additionalLogProperties)
            .CreateLogger();

        return logger;
    }
}