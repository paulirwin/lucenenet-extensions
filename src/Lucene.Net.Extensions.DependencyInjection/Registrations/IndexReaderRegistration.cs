using System;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.DependencyInjection;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    public class IndexReaderRegistration : IDisposable
    {
        private readonly string _name;
        private readonly LuceneDirectory _directory;
        private readonly ServiceLifetime _lifetime;
        private readonly bool _enableRefreshing;
        private DirectoryReader? _cachedReader;
        private readonly object _lock = new();
        private bool _disposed;

        public IndexReaderRegistration(string name, LuceneIndexOptions config, IServiceProvider rootSp)
        {
            _name = name;
            _lifetime = config.ReaderLifetime;
            _enableRefreshing = config.EnableRefreshing;
            _directory = config.DirectoryFactory?.Invoke(rootSp) ?? FSDirectory.Open(config.IndexPath!);
        }

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
