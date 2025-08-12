using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public interface ILuceneBuilder
    {
        ILuceneBuilder AddIndex(string name, Action<LuceneIndexOptions> configure);
    }
}
