using Lucene.Net.Analysis;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public interface IAnalyzerProvider
    {
        Analyzer Get(string name);
    }
}
