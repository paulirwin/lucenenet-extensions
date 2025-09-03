using System;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    /// <summary>
    /// Registers and provides <see cref="DirectoryReader"/> instances with optional caching and refreshing.
    /// Supports different service lifetimes (Singleton, Scoped, Transient) for dependency injection.
    /// </summary>
    public class IndexReaderRegistration : IDisposable
    {
        private readonly string _name;
        private readonly LuceneDirectory _directory;
        private readonly ServiceLifetime _lifetime;
        private readonly bool _enableRefreshing;
        private DirectoryReader? _cachedReader;
        private readonly object _lock = new();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexReaderRegistration"/> class.
        /// </summary>
        /// <param name="name">The unique name of the index reader registration.</param>
        /// <param name="config">Index configuration options.</param>
        /// <param name="rootSp">The root service provider used for directory creation.</param>
        public IndexReaderRegistration(string name, LuceneIndexOptions config, IServiceProvider rootSp)
        {
            _name = name;
            _lifetime = config.ReaderLifetime;
            _enableRefreshing = config.EnableRefreshing;
            _directory = config.DirectoryFactory?.Invoke(rootSp) ?? FSDirectory.Open(config.IndexPath!);
        }

        /// <summary>
        /// Gets a <see cref="DirectoryReader"/> instance according to the configured lifetime.
        /// </summary>
        /// <param name="sp">The service provider to resolve dependencies.</param>
        /// <returns>A <see cref="DirectoryReader"/> instance.</returns>
        /// <exception cref="NotSupportedException">Thrown if the configured service lifetime is unsupported.</exception>
        public DirectoryReader GetReader(IServiceProvider sp)
        {
            return _lifetime switch
            {
                ServiceLifetime.Singleton => _enableRefreshing
                    ? GetSingletonReader()
                    : sp.GetRequiredKeyedService<DirectoryReader>(_name),

                ServiceLifetime.Scoped or ServiceLifetime.Transient =>
                    sp.GetRequiredKeyedService<DirectoryReader>(_name),

                _ => throw new NotSupportedException($"Unsupported lifetime: {_lifetime}")
            };
        }

        // Private helper that returns the cached singleton instance and refreshes it if needed.
        private DirectoryReader GetSingletonReader()
        {
            lock (_lock)
            {
                if (_cachedReader == null)
                {
                    _cachedReader = DirectoryReader.Open(_directory);
                }
                else
                {
                    var maybeUpdated = DirectoryReader.OpenIfChanged(_cachedReader);
                    if (maybeUpdated != null)
                    {
                        _cachedReader.Dispose();
                        _cachedReader = maybeUpdated;
                    }
                }

                return _cachedReader;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="IndexReaderRegistration"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            lock (_lock)
            {
                _cachedReader?.Dispose();
                _cachedReader = null;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
