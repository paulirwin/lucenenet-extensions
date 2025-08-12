using Lucene.Net.Index;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    public class IndexReaderProvider : IIndexReaderProvider
    {
        private readonly IServiceProvider _sp;

        public IndexReaderProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        public IndexReader Get(string name)
        {
            var registration = _sp.GetRequiredKeyedService<IndexReaderRegistration>(name);
            return registration.GetReader(_sp);
        }
    }
}
