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
            var registration = _sp.GetKeyedService<IndexWriterRegistration>(name);
            // Explicit null-check to give a personal clearer error about Writer instead of DI resolution failure.
            if (registration == null)
                throw new InvalidOperationException(
            $"No writer is registered for index '{name}'. " +
            $"Did you forget to configure WriterLifetime?");

            return registration.GetWriter(_sp);
        }
    }
}
