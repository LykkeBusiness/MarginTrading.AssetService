// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Http;

namespace Kathe.LogEnrichment.Correlation.Http
{
    public class HttpCorrelationMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext httpContext, CorrelationContextAccessor contextAccessor)
        {
            if (httpContext.Request.Headers.TryGetValue(HeaderOptions.DefaultCorrelationIdHeader, out var correlationId))
            {
                contextAccessor.CorrelationContext = new CorrelationContext(correlationId);
            }

            await next(httpContext);   
        }
    }
}