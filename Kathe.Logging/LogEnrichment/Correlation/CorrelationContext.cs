// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Kathe.LogEnrichment.Correlation
{
    public class CorrelationContext(string correlationId)
    {
        public string CorrelationId { get; } = correlationId;
    }
}