using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public interface ILuceneBuilder
    {
        IIndexBuilder  AddIndex(string name, Action<LuceneIndexOptions> configure);
    }
}
