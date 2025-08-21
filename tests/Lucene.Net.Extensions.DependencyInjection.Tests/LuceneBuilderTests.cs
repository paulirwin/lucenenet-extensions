using System;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Extensions.DependencyInjection.Providers;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Lucene.Net.Extensions.DependencyInjection.Tests
{
    public class LuceneBuilderTests
    {
        [Fact]
        public void AddLucene_ShouldRegisterProviders()
        {
            var services = new ServiceCollection();

            // Act
            services.AddLucene();

            // Assert
            var sp = services.BuildServiceProvider();
            Assert.NotNull(sp.GetService<IAnalyzerProvider>());
            Assert.NotNull(sp.GetService<IIndexReaderProvider>());
            Assert.NotNull(sp.GetService<IIndexWriterProvider>());
            Assert.NotNull(sp.GetService<IIndexSearcherProvider>());
        }

        [Fact]
        public void AddIndex_ShouldRegisterOptions_AndAnalyzer()
        {
            var services = new ServiceCollection();
            var builder = services.AddLucene();

            builder.AddIndex("TestIndex", options =>
            {
                options.Analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                options.DirectoryFactory = _ => new RAMDirectory(); 
            });

            var sp = services.BuildServiceProvider();

            // Ensure options registered
            var opts = sp.GetRequiredService<IOptionsMonitor<LuceneIndexOptions>>().Get("TestIndex");
            Assert.IsType<StandardAnalyzer>(opts.EffectiveAnalyzer);

            // Ensure analyzer is registered
            var analyzer = sp.GetRequiredKeyedService<Lucene.Net.Analysis.Analyzer>("TestIndex");
            Assert.NotNull(analyzer);
        }

        [Fact]
        public void AddIndexWriter_ShouldRegisterWriterOptions()
        {
            var services = new ServiceCollection();
            var builder = services.AddLucene();

            builder.AddIndex("TestIndex", options =>
            {
                options.DirectoryFactory = _ => new RAMDirectory();
            })
            .AddIndexWriter(options =>
            {
                options.WriterLifetime = ServiceLifetime.Scoped;
            });

            var sp = services.BuildServiceProvider();

            var opts = sp.GetRequiredService<IOptionsMonitor<LuceneWriterOptions>>().Get("TestIndex");
            Assert.Equal(ServiceLifetime.Scoped, opts.WriterLifetime);

            var writer = sp.GetRequiredKeyedService<IndexWriter>("TestIndex");
            Assert.NotNull(writer);
        }

        [Fact]
        public void FluentChaining_ShouldWorkAcrossIndexes()
        {
            var services = new ServiceCollection();
            var builder = services.AddLucene();

            builder.AddIndex("Index1", opts => { opts.DirectoryFactory = _ => new RAMDirectory(); })
                   .AddIndexWriter(w => { })
                   .AddIndex("Index2", opts => { opts.DirectoryFactory = _ => new RAMDirectory(); });

            var sp = services.BuildServiceProvider();

            var index1Writer = sp.GetRequiredKeyedService<IndexWriter>("Index1");
            var index2Analyzer = sp.GetRequiredKeyedService<Lucene.Net.Analysis.Analyzer>("Index2");

            Assert.NotNull(index1Writer);
            Assert.NotNull(index2Analyzer);
        }
    }
}
