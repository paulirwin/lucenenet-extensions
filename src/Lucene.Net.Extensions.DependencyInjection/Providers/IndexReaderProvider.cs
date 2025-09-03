using Lucene.Net.Index;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Provides <see cref="IndexReader"/> instances by name using dependency injection.
    /// </summary>
    public class IndexReaderProvider : IIndexReaderProvider
    {
        private readonly IServiceProvider _sp;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexReaderProvider"/> class.
        /// </summary>
        /// <param name="sp">The service provider used to resolve index readers.</param>
        public IndexReaderProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Gets an <see cref="IndexReader"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index reader.</param>
        /// <returns>An <see cref="IndexReader"/> instance.</returns>
        public IndexReader Get(string name)
        {
            var registration = _sp.GetRequiredKeyedService<IndexReaderRegistration>(name);
            return registration.GetReader(_sp);
        }
    }
}
