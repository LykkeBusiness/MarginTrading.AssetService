using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kathe.LogEnrichment.Outbound
{
    internal class OutboundRequestLoggingHandlerBuilderFilter(ILogger<OutboundRequestLoggingHandler> logger, IOptions<KatheLoggingOptions> options) : IHttpMessageHandlerBuilderFilter
    {
        private readonly KatheLoggingOptions _options = options.Value;

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                if (_options.LogOutgoingRequests)
                {
                    builder.AdditionalHandlers.Add(new OutboundRequestLoggingHandler(logger));
                }

                next(builder);
            };
        }
    }
}