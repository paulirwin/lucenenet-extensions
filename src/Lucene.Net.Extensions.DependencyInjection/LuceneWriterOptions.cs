using System;
using Lucene.Net.Index;
using Microsoft.Extensions.DependencyInjection;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public class LuceneWriterOptions
    {
        public IndexDeletionPolicy? DeletionPolicy { get; set; }
        public ServiceLifetime WriterLifetime { get; set; } = ServiceLifetime.Singleton;

        public Action<IServiceProvider, IndexWriterConfig>? ConfigureIndexWriterConfig { get; set; }

        // Effective fallback
        public IndexDeletionPolicy EffectiveDeletionPolicy =>
            DeletionPolicy ?? new SnapshotDeletionPolicy(new KeepOnlyLastCommitDeletionPolicy());

        // Helper method
        public void ApplyWriterSettings(IServiceProvider sp, IndexWriterConfig config)
        {
            ConfigureIndexWriterConfig?.Invoke(sp, config);

            // Our default
            if (config.MaxBufferedDocs <= 0)
                config.MaxBufferedDocs = 1000;
        }
    }
}
