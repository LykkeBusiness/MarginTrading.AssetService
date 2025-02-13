// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace Kathe.LogEnrichment.Correlation.RabbitMq
{
    public class RabbitMqCorrelationManager(CorrelationContextAccessor contextAccessor)
    {
        public void FetchCorrelationIfExists(IDictionary<string, object> headers)
        {
            if (headers != null &&
                headers.TryGetValue(HeaderOptions.DefaultCorrelationIdHeader, out object value))
            {
                var correlationId = Encoding.UTF8.GetString((byte[]) value);
                contextAccessor.CorrelationContext = new CorrelationContext(correlationId);
            }
            else
            {
                contextAccessor.CorrelationContext = null;
            }
        }

        public IDictionary<string, object> BuildCorrelationHeadersIfExists()
        {
            var correlationId = contextAccessor.CorrelationContext?.CorrelationId;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                return new Dictionary<string, object>
                {
                    { HeaderOptions.DefaultCorrelationIdHeader, correlationId }
                };
            }
            return null;
        }
    }
}