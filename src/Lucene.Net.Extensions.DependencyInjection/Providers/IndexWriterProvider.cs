using Lucene.Net.Index;
using Lucene.Net.Extensions.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Provides <see cref="IndexWriter"/> instances by name using dependency injection.
    /// </summary>
    public class IndexWriterProvider : IIndexWriterProvider
    {
        private readonly IServiceProvider _sp;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexWriterProvider"/> class.
        /// </summary>
        /// <param name="sp">The service provider used to resolve index writers.</param>
        public IndexWriterProvider(IServiceProvider sp)
        {
            _sp = sp;
        }

        /// <summary>
        /// Gets an <see cref="IndexWriter"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the index writer.</param>
        /// <returns>An <see cref="IndexWriter"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no writer is registered for the specified name.</exception>
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
