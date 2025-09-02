using System;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides methods to configure writers for a specific Lucene index.
    /// </summary>
    public interface IIndexBuilder : ILuceneBuilder
    {
        IIndexBuilder AddIndexWriter(Action<LuceneWriterOptions> configure);
    }
}
