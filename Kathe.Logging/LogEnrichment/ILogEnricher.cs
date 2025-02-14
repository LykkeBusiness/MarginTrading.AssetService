namespace Kathe.LogEnrichment;

public interface ILogEnricher
{
    void Enrich(IDictionary<string, object> state);
}