using Lucene.Net.Search;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public interface IIndexSearcherProvider
    {
        IndexSearcher Get(string name);
    }
}
