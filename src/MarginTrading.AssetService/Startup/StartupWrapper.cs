using Kathe;
using Kathe.Configuration;

using Lykke.Snow.Common.Startup;

using Serilog;
using Serilog.Core;

namespace MarginTrading.AssetService.Startup
{
    internal class StartupWrapper
    {
        private readonly Logger _logger = new LoggerConfiguration()
            .AddDefaultConfig(Program.ApplicationName)
            .CreateLogger();

        public Task StartAsync(Func<Task> startAction)
        {
            return StartupLoggingWrapper.HandleStartupException(async () =>
            {
                FailureWrapper.InitializeForHostRestart();
                
                await FailureWrapper.RetryAsync(startAction, LogStartupException);
            }, Program.ApplicationName);
        }
        
        private void LogStartupException(Exception e, uint attemptLeft)
        {
            _logger.Fatal(e, "Host restart initiated. Attempts left: {attemptsLeft}", attemptLeft);
        }
    }
}