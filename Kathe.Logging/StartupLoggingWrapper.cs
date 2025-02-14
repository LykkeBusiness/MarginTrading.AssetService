using System.Reflection;

using Kathe.Configuration;

using Serilog;

namespace Kathe;

public static class StartupLoggingWrapper
{
    public static async Task HandleStartupException(Func<Task> startup, string serviceShortName)
    {
        var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName() ?? new AssemblyName("Unknown");
        serviceShortName ??= entryAssemblyName.Name;

        var logger = new LoggerConfiguration().AddDefaultConfig(serviceShortName).CreateLogger();

        try
        {
            logger.Information("{Name} version {Version}, env {EnvInfo}", entryAssemblyName, entryAssemblyName.Version?.ToString(), Environment.GetEnvironmentVariable("ENV_INFO"));
                
            await startup();
        }
        catch (Exception e)
        {
            logger.Fatal(e, "Host terminated unexpectedly");
                
            // Lets devops to see startup error in console between restarts in the Kubernetes
            var delay = TimeSpan.FromMinutes(1);

            logger.Information("Process will be terminated in {Delay}. Press any key to terminate immediately", delay);

            await Task.WhenAny(
                Task.Delay(delay),
                Task.Run(() => Console.ReadKey(true)));
                
            logger.Information("Terminated");
        }
    }
}