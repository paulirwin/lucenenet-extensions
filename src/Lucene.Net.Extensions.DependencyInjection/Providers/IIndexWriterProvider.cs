using Lucene.Net.Index;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Interface for providing <see cref="IndexWriter"/> instances by name.
    /// </summary>
    public interface IIndexWriterProvider
    {
        /// <summary>
        /// Gets an <see cref="IndexWriter"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index writer.</param>
        /// <returns>An <see cref="IndexWriter"/> instance.</returns>
        IndexWriter Get(string name);
    }
}
