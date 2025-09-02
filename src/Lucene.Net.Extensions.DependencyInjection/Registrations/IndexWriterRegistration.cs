using System;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    /// <summary>
    /// Registers and provides <see cref="IndexWriter"/> instances with optional singleton caching.
    /// Supports different service lifetimes (Singleton, Scoped, Transient) for dependency injection.
    /// </summary>
    public class IndexWriterRegistration : IDisposable
    {
        private readonly string _name;
        private readonly LuceneDirectory _directory;
        private readonly IndexWriterConfig _writerConfig;
        private readonly ServiceLifetime _lifetime;

        private IndexWriter? _cachedWriter;
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexWriterRegistration"/> class.
        /// </summary>
        /// <param name="name">The unique name of the index writer registration.</param>
        /// <param name="indexConfig">Index configuration options.</param>
        /// <param name="writerConfig">Writer-specific configuration options.</param>
        /// <param name="rootSp">The root service provider used for directory creation.</param>
        public IndexWriterRegistration(
            string name,
            LuceneIndexOptions indexConfig,
            LuceneWriterOptions writerConfig,
            IServiceProvider rootSp)
        {
            _name = name;
            _lifetime = writerConfig.WriterLifetime;

            _directory = indexConfig.DirectoryFactory?.Invoke(rootSp)
                         ?? FSDirectory.Open(indexConfig.IndexPath!);

            _writerConfig = new IndexWriterConfig(indexConfig.LuceneVersion, indexConfig.EffectiveAnalyzer)
            {
                IndexDeletionPolicy = writerConfig.EffectiveDeletionPolicy
            };

            writerConfig.ApplyWriterSettings(rootSp, _writerConfig);
        }

        /// <summary>
        /// Gets an <see cref="IndexWriter"/> instance according to the configured lifetime.
        /// </summary>
        /// <param name="sp">The service provider to resolve dependencies.</param>
        /// <returns>An <see cref="IndexWriter"/> instance.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a singleton writer cannot obtain a lock (e.g., another process is writing to the index).
        /// </exception>
        /// <exception cref="NotSupportedException">Thrown if the configured service lifetime is unsupported.</exception>
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

        // Private helper that returns the cached singleton instance and refreshes it if needed.
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

        /// <summary>
        /// Releases all resources used by the <see cref="IndexWriterRegistration"/>.
        /// </summary>
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
