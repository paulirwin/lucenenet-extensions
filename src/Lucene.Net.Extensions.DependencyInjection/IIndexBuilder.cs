using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public interface IIndexBuilder : ILuceneBuilder
    {
        IIndexBuilder AddIndexWriter(Action<LuceneWriterOptions> configure);
    }
}
