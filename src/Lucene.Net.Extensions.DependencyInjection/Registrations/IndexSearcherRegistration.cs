using System;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Registrations
{
    public class IndexSearcherRegistration : IDisposable
    {
        private readonly string _name;
        private readonly ServiceLifetime _lifetime;
        private IndexSearcher? _cachedSearcher;
        private readonly object _lock = new();
        private bool _disposed;

        public IndexSearcherRegistration(string name, LuceneIndexOptions config)
        {
            _name = name;
            _lifetime = config.SearcherLifetime;
        }

        public IndexSearcher GetSearcher(IServiceProvider sp)
        {
            return _lifetime switch
            {
                ServiceLifetime.Singleton => GetSingletonSearcher(sp),
                ServiceLifetime.Scoped or ServiceLifetime.Transient => sp.GetRequiredKeyedService<IndexSearcher>(_name),
                _ => throw new NotSupportedException($"Unsupported lifetime: {_lifetime}")
            };
        }

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
