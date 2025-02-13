// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Serilog.Core;
using Serilog.Events;

namespace Kathe.LogEnrichment.Correlation.Serilog
{
    
    public class CorrelationLogEventEnricher(CorrelationContextAccessor correlationContextAccessor, string propertyName = "CorrelationId") : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var correlationId = correlationContextAccessor.CorrelationContext?.CorrelationId;
            var property = propertyFactory.CreateProperty(propertyName, correlationId);
            logEvent.AddOrUpdateProperty(property);
        }
    }
}