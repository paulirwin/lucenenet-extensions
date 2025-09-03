using System;
using Lucene.Net.Extensions.DependencyInjection.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for registering Lucene.NET services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class LuceneDIExtensions
    {
        /// <summary>
        /// Adds Lucene.NET services (readers, writers, searchers, analyzers) to the DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>An <see cref="ILuceneBuilder"/> to allow fluent registration of indexes and writers.</returns>
        public static ILuceneBuilder AddLucene(this IServiceCollection services)
        {
            services.AddTransient<IIndexReaderProvider, IndexReaderProvider>();
            services.AddTransient<IIndexWriterProvider, IndexWriterProvider>();
            services.AddTransient<IIndexSearcherProvider, IndexSearcherProvider>();
            services.AddSingleton<IAnalyzerProvider, AnalyzerProvider>();

            return new LuceneBuilder(services);
        }
    }
}
