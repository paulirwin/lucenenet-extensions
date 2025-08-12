using Lucene.Net.Analysis;
using Microsoft.Extensions.DependencyInjection;


namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public class AnalyzerProvider : IAnalyzerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        public AnalyzerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Analyzer Get(string name)
        {
            return _serviceProvider.GetRequiredKeyedService<Analyzer>(name);
        }
    }
}
