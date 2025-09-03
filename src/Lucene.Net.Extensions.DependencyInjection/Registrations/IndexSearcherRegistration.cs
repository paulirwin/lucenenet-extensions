using System;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    /// <summary>
    /// Registers and provides <see cref="IndexSearcher"/> instances with optional singleton caching.
    /// Supports different service lifetimes (Singleton, Scoped, Transient) for dependency injection.
    /// </summary>
    public class IndexSearcherRegistration : IDisposable
    {
        private readonly string _name;
        private readonly ServiceLifetime _lifetime;
        private IndexSearcher? _cachedSearcher;
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSearcherRegistration"/> class.
        /// </summary>
        /// <param name="name">The unique name of the index searcher registration.</param>
        /// <param name="config">Index configuration options.</param>
        public IndexSearcherRegistration(string name, LuceneIndexOptions config)
        {
            _name = name;
            _lifetime = config.SearcherLifetime;
        }

        /// <summary>
        /// Gets an <see cref="IndexSearcher"/> instance according to the configured lifetime.
        /// </summary>
        /// <param name="sp">The service provider to resolve dependencies.</param>
        /// <returns>An <see cref="IndexSearcher"/> instance.</returns>
        /// <exception cref="NotSupportedException">Thrown if the configured service lifetime is unsupported.</exception>
        public IndexSearcher GetSearcher(IServiceProvider sp)
        {
            return _lifetime switch
            {
                ServiceLifetime.Singleton => GetSingletonSearcher(sp),
                ServiceLifetime.Scoped or ServiceLifetime.Transient => sp.GetRequiredKeyedService<IndexSearcher>(_name),
                _ => throw new NotSupportedException($"Unsupported lifetime: {_lifetime}")
            };
        }

        // Private helper that returns the cached singleton instance and refreshes it if needed.
        private IndexSearcher GetSingletonSearcher(IServiceProvider sp)
        {
            lock (_lock)
            {
                var readerReg = sp.GetRequiredKeyedService<IndexReaderRegistration>(_name);
                var currentReader = readerReg.GetReader(sp);

                if (_cachedSearcher?.IndexReader != currentReader)
                {
                    _cachedSearcher = new IndexSearcher(currentReader);
                }

                return _cachedSearcher;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="IndexSearcherRegistration"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            lock (_lock)
            {
                _cachedSearcher = null;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
