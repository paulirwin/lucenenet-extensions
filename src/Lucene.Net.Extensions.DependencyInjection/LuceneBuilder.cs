using Lucene.Net.Analysis;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public class LuceneBuilder : ILuceneBuilder
    {
        private readonly IServiceCollection _services;

        public LuceneBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Register a Lucene index (readers, searchers, analyzers, etc.).
        /// </summary>
        public IIndexBuilder AddIndex(string name, Action<LuceneIndexOptions> configure)
        {
            var options = new LuceneIndexOptions();
            configure(options);

            _services.Configure<LuceneIndexOptions>(name, configure); // still preserve this for later use

            // Register IndexReader
            if (options.ReaderLifetime == ServiceLifetime.Singleton)
                _services.AddKeyedSingleton<DirectoryReader>(name, CreateIndexReader);
            else if (options.ReaderLifetime == ServiceLifetime.Scoped)
                _services.AddKeyedScoped<DirectoryReader>(name, CreateIndexReader);
            else
                _services.AddKeyedTransient<DirectoryReader>(name, CreateIndexReader);

            // Register IndexSearcher
            if (options.SearcherLifetime == ServiceLifetime.Singleton)
                _services.AddKeyedSingleton<IndexSearcher>(name, CreateIndexSearcher);
            else if (options.SearcherLifetime == ServiceLifetime.Scoped)
                _services.AddKeyedScoped<IndexSearcher>(name, CreateIndexSearcher);
            else
                _services.AddKeyedTransient<IndexSearcher>(name, CreateIndexSearcher);

            // Register Registrations
            _services.AddKeyedSingleton<IndexReaderRegistration>(name, (sp, _) => new IndexReaderRegistration(name, options, sp));
            _services.AddKeyedSingleton<IndexSearcherRegistration>(name, (sp, _) => new IndexSearcherRegistration(name, options));
            _services.AddKeyedSingleton<Analyzer>(name, (sp, _) => options.EffectiveAnalyzer);

            return new IndexBuilder(this, name);
        }

        /// <summary>
        /// Register IndexWriter for a given index.
        /// </summary>
        internal void AddIndexWriter(string name, Action<LuceneWriterOptions> configure)
        {
            var options = new LuceneWriterOptions();
            configure(options);

            _services.Configure<LuceneWriterOptions>(name, configure);

            if (options.WriterLifetime == ServiceLifetime.Singleton)
                _services.AddKeyedSingleton<IndexWriter>(name, CreateIndexWriter);
            else if (options.WriterLifetime == ServiceLifetime.Scoped)
                _services.AddKeyedScoped<IndexWriter>(name, CreateIndexWriter);
            else
                _services.AddKeyedTransient<IndexWriter>(name, CreateIndexWriter);

            _services.AddKeyedSingleton<IndexWriterRegistration>(name,
                (sp, _) =>
                 {
                     var indexOptions = sp.GetRequiredService<IOptionsMonitor<LuceneIndexOptions>>().Get(name);
                     var writerOpts = sp.GetRequiredService<IOptionsMonitor<LuceneWriterOptions>>().Get(name);

                     return new IndexWriterRegistration(name, indexOptions, writerOpts, sp);
                 });
        }

        // ----------------- Factories -----------------
        private static DirectoryReader CreateIndexReader(IServiceProvider sp, object? key)
        {
            var name = (string)key!;
            var config = sp.GetRequiredService<IOptionsMonitor<LuceneIndexOptions>>().Get(name);
            var directory = config.DirectoryFactory?.Invoke(sp) ?? FSDirectory.Open(config.IndexPath!);
            return DirectoryReader.Open(directory);
        }
        private static IndexWriter CreateIndexWriter(IServiceProvider sp, object? key)
        {
            var name = (string)key!;
            var indexConfig = sp.GetRequiredService<IOptionsMonitor<LuceneIndexOptions>>().Get(name);
            var writerConfig = sp.GetRequiredService<IOptionsMonitor<LuceneWriterOptions>>().Get(name);

            var directory = indexConfig.DirectoryFactory?.Invoke(sp) ?? FSDirectory.Open(indexConfig.IndexPath!);

            var iwc = new IndexWriterConfig(indexConfig.LuceneVersion, indexConfig.EffectiveAnalyzer)
            {
                IndexDeletionPolicy = writerConfig.EffectiveDeletionPolicy
            };

            writerConfig.ApplyWriterSettings(sp, iwc);

            return new IndexWriter(directory, iwc);
        }
        private static IndexSearcher CreateIndexSearcher(IServiceProvider sp, object? key)
        {
            var name = (string)key!;
            var readerRegistration = sp.GetRequiredKeyedService<IndexReaderRegistration>(name);

            return new IndexSearcher(readerRegistration.GetReader(sp));
        }

    }
}
