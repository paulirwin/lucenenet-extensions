using System;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    public class IndexWriterRegistration : IDisposable
    {
        private readonly string _name;
        private readonly LuceneDirectory _directory;
        private readonly IndexWriterConfig _writerConfig;
        private readonly ServiceLifetime _lifetime;

        private IndexWriter? _cachedWriter;
        private readonly object _lock = new();
        private bool _disposed;

        public IndexWriterRegistration(string name, LuceneIndexOptions config, IServiceProvider rootSp)
        {
            _name = name;
            _lifetime = config.WriterLifetime;

            _directory = config.DirectoryFactory?.Invoke(rootSp)
                         ?? FSDirectory.Open(config.IndexPath!);

            _writerConfig = new IndexWriterConfig(config.LuceneVersion, config.EffectiveAnalyzer)
            {
                IndexDeletionPolicy = config.EffectiveDeletionPolicy
            };

            config.ApplyWriterSettings(rootSp, _writerConfig);
        }

        public IndexWriter GetWriter(IServiceProvider sp)
        {
            return _lifetime switch
            {
                ServiceLifetime.Singleton => GetSingletonWriter(),
                ServiceLifetime.Scoped or ServiceLifetime.Transient =>
                    sp.GetRequiredKeyedService<IndexWriter>(_name),
                _ => throw new NotSupportedException($"Unsupported lifetime: {_lifetime}")
            };
        }

        private IndexWriter GetSingletonWriter()
        {
            if (_cachedWriter != null) return _cachedWriter;

            lock (_lock)
            {
                if (_cachedWriter == null)
                {
                    try
                    {
                        _cachedWriter = new IndexWriter(_directory, _writerConfig);
                    }
                    catch (LockObtainFailedException ex)
                    {
                        throw new InvalidOperationException(
                            "IndexWriter lock failed. Is another process writing to the index?",
                            ex
                        );
                    }
                }
            }

            return _cachedWriter!;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _cachedWriter?.Dispose();
            _cachedWriter = null;

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
