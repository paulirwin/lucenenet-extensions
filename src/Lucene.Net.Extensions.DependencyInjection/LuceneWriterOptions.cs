using System;
using Lucene.Net.Index;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    /// <summary>
    /// Configuration options for a Lucene <see cref="IndexWriter"/>.
    /// </summary>
    public class LuceneWriterOptions
    {
        public IndexDeletionPolicy? DeletionPolicy { get; set; }
        public ServiceLifetime WriterLifetime { get; set; } = ServiceLifetime.Singleton;

        public Action<IServiceProvider, IndexWriterConfig>? ConfigureIndexWriterConfig { get; set; }

        // Effective fallback
        public IndexDeletionPolicy EffectiveDeletionPolicy =>
            DeletionPolicy ?? new SnapshotDeletionPolicy(new KeepOnlyLastCommitDeletionPolicy());

        /// <summary>
        /// Applies the configured writer settings to the given <see cref="IndexWriterConfig"/>.
        /// </summary>
        /// <param name="sp">The DI service provider.</param>
        /// <param name="config">The <see cref="IndexWriterConfig"/> to apply settings to.</param>
        public void ApplyWriterSettings(IServiceProvider sp, IndexWriterConfig config)
        {
            ConfigureIndexWriterConfig?.Invoke(sp, config);

            // Apply default
            if (config.MaxBufferedDocs <= 0)
                config.MaxBufferedDocs = 1000;
        }
    }
}
