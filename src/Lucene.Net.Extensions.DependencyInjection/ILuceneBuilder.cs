using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides methods to register Lucene indexes and related services (readers, searchers, writers, analyzers).
    /// </summary>
    public interface ILuceneBuilder
    {
        /// <summary>
        /// Adds a new Lucene index with the specified name and configuration options.
        /// </summary>
        /// <param name="name">The unique name of the index.</param>
        /// <param name="configure">A delegate to configure <see cref="LuceneIndexOptions"/>.</param>
        /// <returns>An <see cref="IIndexBuilder"/> for chaining index-specific registrations.</returns>
        IIndexBuilder AddIndex(string name, Action<LuceneIndexOptions> configure);
    }
}
