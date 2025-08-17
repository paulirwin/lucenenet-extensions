using Lucene.Net.Index;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public interface IIndexReaderProvider
    {
        IndexReader Get(string name);
    }
}
