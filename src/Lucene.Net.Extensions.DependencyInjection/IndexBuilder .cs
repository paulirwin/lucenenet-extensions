using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    internal class IndexBuilder : IIndexBuilder
    {
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
