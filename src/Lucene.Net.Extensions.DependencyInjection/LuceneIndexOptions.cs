using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Microsoft.Extensions.DependencyInjection;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.DependencyInjection
{
    public class LuceneIndexOptions
    {
        public string? IndexPath { get; set; }
        public Func<IServiceProvider, LuceneDirectory>? DirectoryFactory { get; set; }

        public Analyzer? Analyzer { get; set; }
        public LuceneVersion LuceneVersion { get; set; } = LuceneVersion.LUCENE_48;

        public bool EnableRefreshing { get; set; } = false;

        public IndexDeletionPolicy? DeletionPolicy { get; set; }

        public Action<IServiceProvider, IndexWriterConfig>? ConfigureIndexWriterConfig { get; set; }

        public ServiceLifetime ReaderLifetime { get; set; } = ServiceLifetime.Singleton;
        public ServiceLifetime WriterLifetime { get; set; } = ServiceLifetime.Singleton;
        public ServiceLifetime SearcherLifetime { get; set; } = ServiceLifetime.Singleton;

        // Effective fallbacks
        public Analyzer EffectiveAnalyzer => Analyzer ?? new StandardAnalyzer(LuceneVersion);
        public IndexDeletionPolicy EffectiveDeletionPolicy =>
            DeletionPolicy ?? new SnapshotDeletionPolicy(new KeepOnlyLastCommitDeletionPolicy());

        // ✅ Helper method
        public void ApplyWriterSettings(IServiceProvider sp, IndexWriterConfig config)
        {
            ConfigureIndexWriterConfig?.Invoke(sp, config);
        }


    }
}
