using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Implements <see cref="IIndexBuilder"/> to allow configuring writers for a specific index.
    /// Delegates back to <see cref="LuceneBuilder"/> for adding more indexes.
    /// </summary>
    internal class IndexBuilder : IIndexBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IndexBuilder"/>.
        /// </summary>
        /// <param name="luceneBuilder">The main <see cref="LuceneBuilder"/> instance.</param>
        /// <param name="indexName">The name of the index being configured.</param>
        private readonly LuceneBuilder _luceneBuilder;
        private readonly string _indexName;

        public IndexBuilder(LuceneBuilder luceneBuilder, string indexName)
        {
            _luceneBuilder = luceneBuilder;
            _indexName = indexName;
        }

        public IIndexBuilder AddIndexWriter(Action<LuceneWriterOptions> configure)
        {
            _luceneBuilder.AddIndexWriter(_indexName, configure);
            return this; // keep chaining on the same index
        }

        // delegate back to LuceneBuilder for adding more indexes
        public IIndexBuilder AddIndex(string name, Action<LuceneIndexOptions> configure)
        {
            return _luceneBuilder.AddIndex(name, configure);
        }
    }
}
