// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Kathe.LogEnrichment.Correlation.Http
{
    public sealed class HttpCorrelationHandler(CorrelationContextAccessor contextAccessor) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = contextAccessor.CorrelationContext?.CorrelationId;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                request.Headers.Add(HeaderOptions.DefaultCorrelationIdHeader, correlationId);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}