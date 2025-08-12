using System;
using Lucene.Net.Extensions.DependencyInjection.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public static class LuceneDIExtensions
    {
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