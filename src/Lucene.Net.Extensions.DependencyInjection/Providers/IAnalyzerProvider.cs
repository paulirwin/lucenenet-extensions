using Lucene.Net.Analysis;

namespace Lucene.Net.Extensions.DependencyInjection.Providers
{
    /// <summary>
    /// Interface for providing <see cref="Analyzer"/> instances by name.
    /// </summary>
    public interface IAnalyzerProvider
    {
        /// <summary>
        /// Gets an <see cref="Analyzer"/> instance by its registered name.
        /// </summary>
        /// <param name="name">The name of the analyzer.</param>
        /// <returns>An <see cref="Analyzer"/> instance.</returns>
        Analyzer Get(string name);
    }
}
