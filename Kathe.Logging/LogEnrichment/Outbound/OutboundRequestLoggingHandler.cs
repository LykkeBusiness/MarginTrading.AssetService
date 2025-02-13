using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Kathe.LogEnrichment.Outbound
{
    public class OutboundRequestLoggingHandler(ILogger<OutboundRequestLoggingHandler> log) : DelegatingHandler
    {
        private readonly UrlQueryStringHider _queryStringHider = new(log);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                log.LogDebug("Started executing {method} on {url}",
                    request.Method,
                    request.RequestUri?.GetLeftPart(UriPartial.Scheme | UriPartial.Authority | UriPartial.Path));

                var result = await base.SendAsync(request, cancellationToken);

                LogLevel logLevel;

                if (stopwatch.Elapsed > TimeSpan.FromSeconds(1))
                {
                    logLevel = LogLevel.Warning;
                }
                else if ((int) result.StatusCode >= 500)
                {
                    logLevel = LogLevel.Warning;
                }
                else if ((int) result.StatusCode >= 400 || (int) result.StatusCode < 200)
                {
                    logLevel = LogLevel.Information;
                }
                else
                {
                    logLevel = LogLevel.Debug;
                }

                log.Log(logLevel, "Executing {method} on {url} returned {statuscode} in {ms} ms",
                    request.Method,
                    _queryStringHider.HideQuerystringValues(request.RequestUri),
                    result.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (OperationCanceledException)
            {
                log.LogWarning("Executing {method} on {url} was cancelled in {ms} ms",
                    request.Method,
                    _queryStringHider.HideQuerystringValues(request.RequestUri),
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "Exception while executing {method} on {url} in {ms} ms",
                    request.Method,
                    _queryStringHider.HideQuerystringValues(request.RequestUri),
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}