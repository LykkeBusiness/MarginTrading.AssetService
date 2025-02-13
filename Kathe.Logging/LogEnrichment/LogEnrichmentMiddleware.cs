using System.Diagnostics;

using Kathe.LogEnrichment.Outbound;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kathe.LogEnrichment
{
    /// <summary>
    /// Middleware that enriches the log context with additional messages
    /// </summary>
    public class LogEnrichmentMiddleware(
        RequestDelegate next,
        ILogger<LogEnrichmentMiddleware> log,
        IEnumerable<ILogEnricher> logEnrichers,
        IOptions<KatheLoggingOptions> options)
    {
        private readonly KatheLoggingOptions _options = options.Value;
        private readonly UrlQueryStringHider _queryStringHider = new(log);

        public async Task Invoke(HttpContext context)
        {
            if (!_options.LogIncomingRequests)
            {
                await next(context);
                return;
            }

            var state = new Dictionary<string, object>();

            foreach (var logEnricher in logEnrichers)
            {
                logEnricher.Enrich(state);
            }

            using (log.BeginScope(state))
            {
                var stopwatch = Stopwatch.StartNew();
                
                await next(context);

                LogLevel logLevel = stopwatch.Elapsed > TimeSpan.FromSeconds(1)
                    ? LogLevel.Warning
                    : context.Response.StatusCode switch
                    {
                        >= 500 => LogLevel.Warning,
                        >= 400 or < 200 => LogLevel.Information,
                        _ => LogLevel.Debug
                    };
                
                var path = _queryStringHider.HideQuerystringValues(context.Request.GetEncodedPathAndQuery());
                var bytes = "N/A";
                try
                {
                    bytes = context.Response.Body.Length.ToString();
                }
                catch
                {
                    // suppress if can't get
                }

#pragma warning disable CA2254
                log.Log(logLevel, $"{context.Request.Method} {path} responded {context.Response.StatusCode} in {stopwatch.Elapsed.TotalMilliseconds}ms ({bytes} bytes) ");
#pragma warning restore CA2254
            }
        }
    }
}
