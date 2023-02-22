using System;
using System.Threading.Tasks;

using Lykke.Logs.Serilog;
using Lykke.Snow.Common.Startup;

using Serilog;

namespace MarginTrading.AssetService.Startup
{
    internal static class StartupWrapper
    {
        public static Task StartAsync(Func<Task> startAction)
        {
            return StartupLoggingWrapper.HandleStartupException(async () =>
            {
                FailureWrapper.InitializeForHostRestart();

                await FailureWrapper.RetryAsync(startAction, LogStartupException);
            }, "asset");
        }
        
        private static void LogStartupException(Exception e, uint attemptLeft)
        {
            Log.Fatal(e, "Host restart initiated. Attempts left: {attemptsLeft}", attemptLeft);
        }
    }
}