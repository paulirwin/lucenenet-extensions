using Lucene.Net.Search;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Interface for providing <see cref="IndexSearcher"/> instances by name.
    /// </summary>
    public interface IIndexSearcherProvider
    {
        /// <summary>
        /// Gets an <see cref="IndexSearcher"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index searcher.</param>
        /// <returns>An <see cref="IndexSearcher"/> instance.</returns>
        IndexSearcher Get(string name);
    }
}
