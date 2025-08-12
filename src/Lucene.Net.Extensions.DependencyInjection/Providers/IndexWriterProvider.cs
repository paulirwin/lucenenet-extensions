using Lucene.Net.Index;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public class IndexWriterProvider : IIndexWriterProvider
    {
        private readonly IServiceProvider _sp;

        public IndexWriterProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        public IndexWriter Get(string name)
        {
            var registration = _sp.GetRequiredKeyedService<IndexWriterRegistration>(name);
            return registration.GetWriter(_sp);
        }
    }
}
