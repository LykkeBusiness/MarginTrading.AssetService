// namespace Kathe.LogEnrichment.AspNetCore.Correlation;
//
// public class CorrelationIdLogsEnricher(CorrelationContextAccessor contextAccessor) : ILogEnricher
// {
//     public void Enrich(IDictionary<string, object> state)
//     {
//         var context = contextAccessor.CorrelationContext;
//         state.TryAdd(HeaderOptions.DefaultCorrelationIdHeader, context.CorrelationId);
//     }
// }