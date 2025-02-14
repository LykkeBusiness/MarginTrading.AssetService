// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Kathe.LogEnrichment.Correlation.Cqrs
{
    public class CqrsCorrelationManager(CorrelationContextAccessor contextAccessor)
    {
        public void FetchCorrelationIfExists(IDictionary<string, string> headers)
        {
            if (headers != null &&
                headers.TryGetValue(HeaderOptions.DefaultCorrelationIdHeader, out string correlationId))
            {
                contextAccessor.CorrelationContext = new CorrelationContext(correlationId);
            }
            else
            {
                contextAccessor.CorrelationContext = null;
            }
        }

        public IDictionary<string, string> BuildCorrelationHeadersIfExists()
        {
            var correlationId = contextAccessor.CorrelationContext?.CorrelationId;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                return new Dictionary<string, string>
                {
                    { HeaderOptions.DefaultCorrelationIdHeader, correlationId }
                };
            }
            return null;
        }
    }
}