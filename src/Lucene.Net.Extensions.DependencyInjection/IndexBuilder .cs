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
            return this;
        }
    }
}