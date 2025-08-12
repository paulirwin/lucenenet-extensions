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

        public ILuceneBuilder AddIndex(string name, Action<LuceneIndexOptions> configure)
        {
            var options = new LuceneIndexOptions();
            configure(options);

            var existingConfig = options.ConfigureIndexWriterConfig;

            options.ConfigureIndexWriterConfig = (sp, config) =>
            {
                existingConfig?.Invoke(sp, config);

                if (config.MaxBufferedDocs <= 0)
                {
                    config.MaxBufferedDocs = 1000;
                }
            };


            _services.Configure<LuceneIndexOptions>(name, configure); // still preserve this for later use

            // Register IndexReader
            if (options.ReaderLifetime == ServiceLifetime.Singleton)
                _services.AddKeyedSingleton<DirectoryReader>(name, CreateIndexReader);
            else if (options.ReaderLifetime == ServiceLifetime.Scoped)
                _services.AddKeyedScoped<DirectoryReader>(name, CreateIndexReader);
            else
                _services.AddKeyedTransient<DirectoryReader>(name, CreateIndexReader);

            // Register IndexWriter
            if (options.WriterLifetime == ServiceLifetime.Singleton)
                _services.AddKeyedSingleton<IndexWriter>(name, CreateIndexWriter);
            else if (options.WriterLifetime == ServiceLifetime.Scoped)
                _services.AddKeyedScoped<IndexWriter>(name, CreateIndexWriter);
            else
                _services.AddKeyedTransient<IndexWriter>(name, CreateIndexWriter);

            // Register IndexSearcher
            if (options.SearcherLifetime == ServiceLifetime.Singleton)
                _services.AddKeyedSingleton<IndexSearcher>(name, CreateIndexSearcher);
            else if (options.SearcherLifetime == ServiceLifetime.Scoped)
                _services.AddKeyedScoped<IndexSearcher>(name, CreateIndexSearcher);
            else
                _services.AddKeyedTransient<IndexSearcher>(name, CreateIndexSearcher);

            // Register Registrations
            _services.AddKeyedSingleton<IndexReaderRegistration>(name, (sp, _) => new IndexReaderRegistration(name, options, sp));
            _services.AddKeyedSingleton<IndexWriterRegistration>(name, (sp, _) => new IndexWriterRegistration(name, options, sp));
            _services.AddKeyedSingleton<IndexSearcherRegistration>(name, (sp, _) => new IndexSearcherRegistration(name, options));
            _services.AddKeyedSingleton<Analyzer>(name, (sp, _) => options.EffectiveAnalyzer);

            return this;
        }

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
            var config = sp.GetRequiredService<IOptionsMonitor<LuceneIndexOptions>>().Get(name);

            var directory = config.DirectoryFactory?.Invoke(sp) ?? FSDirectory.Open(config.IndexPath!);

            var writerConfig = new IndexWriterConfig(config.LuceneVersion, config.EffectiveAnalyzer)
            {
                IndexDeletionPolicy = config.EffectiveDeletionPolicy
            };

            config.ApplyWriterSettings(sp, writerConfig);

            return new IndexWriter(directory, writerConfig);
        }


        private static IndexSearcher CreateIndexSearcher(IServiceProvider sp, object? key)
        {
            var name = (string)key!;
            var readerRegistration = sp.GetRequiredKeyedService<IndexReaderRegistration>(name);

            return new IndexSearcher(readerRegistration.GetReader(sp));
        }

    }
}
