using Lucene.Net.Index;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public interface IIndexWriterProvider
    {
        IndexWriter Get(string name);
    }
}
