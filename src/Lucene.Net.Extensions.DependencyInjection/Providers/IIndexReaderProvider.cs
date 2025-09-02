using Lucene.Net.Index;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Interface for providing <see cref="IndexReader"/> instances by name.
    /// </summary>
    public interface IIndexReaderProvider
    {
        /// <summary>
        /// Gets an <see cref="IndexReader"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index reader.</param>
        /// <returns>An <see cref="IndexReader"/> instance.</returns>
        IndexReader Get(string name);
    }
}
