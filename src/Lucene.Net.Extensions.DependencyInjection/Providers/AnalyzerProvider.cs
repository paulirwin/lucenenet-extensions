using Lucene.Net.Analysis;
using Microsoft.Extensions.DependencyInjection;


namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Provides <see cref="Analyzer"/> instances by name using dependency injection.
    /// </summary>
    public class AnalyzerProvider : IAnalyzerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerProvider"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve analyzers.</param>
        public AnalyzerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets an <see cref="Analyzer"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the analyzer.</param>
        /// <returns>An <see cref="Analyzer"/> instance.</returns>
        public Analyzer Get(string name)
        {
            return _serviceProvider.GetRequiredKeyedService<Analyzer>(name);
        }
    }
}
