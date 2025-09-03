using Lucene.Net.Search;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Provides <see cref="IndexSearcher"/> instances by name using dependency injection.
    /// </summary>
    public class IndexSearcherProvider : IIndexSearcherProvider
    {
        private readonly IServiceProvider _sp;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSearcherProvider"/> class.
        /// </summary>
        /// <param name="sp">The service provider used to resolve index searchers.</param>
        public IndexSearcherProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Gets an <see cref="IndexSearcher"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index searcher.</param>
        /// <returns>An <see cref="IndexSearcher"/> instance.</returns>
        public IndexSearcher Get(string name)
        {
            var registration = _sp.GetRequiredKeyedService<IndexSearcherRegistration>(name);
            return registration.GetSearcher(_sp);
        }
    }
}
