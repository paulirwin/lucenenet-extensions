using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public interface IIndexBuilder
    {
         IIndexBuilder AddIndexWriter(Action<LuceneWriterOptions> configure);
    }
}