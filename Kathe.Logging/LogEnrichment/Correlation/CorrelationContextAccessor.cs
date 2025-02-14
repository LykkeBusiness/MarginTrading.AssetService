// Copyright (c) 2020 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Kathe.LogEnrichment.Correlation
{
    public class CorrelationContextAccessor
    {
        private readonly bool _generateCorrelationIdIfEmpty =
            Environment.GetEnvironmentVariable("GenerateCorrelationIdIfEmpty") == "true";
        private static readonly AsyncLocal<CorrelationContextHolder> CorrelationContextCurrent = new AsyncLocal<CorrelationContextHolder>();

        public CorrelationContext CorrelationContext
        {
            get
            {
                if (CorrelationContextCurrent.Value == null && _generateCorrelationIdIfEmpty)
                {
                    var correlationId = Guid.NewGuid().ToString("N");
                    CorrelationContextCurrent.Value = new CorrelationContextHolder
                    {
                        Context = new CorrelationContext(correlationId)
                    };
                }
                return CorrelationContextCurrent.Value?.Context;
            }
            set
            {
                var holder = CorrelationContextCurrent.Value;
                if (holder != null)
                {
                    // Clear current CorrelationContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the CorrelationContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    CorrelationContextCurrent.Value = new CorrelationContextHolder { Context = value };
                }
            }
        }

        private class CorrelationContextHolder
        {
            public CorrelationContext Context;
        }
    }
}