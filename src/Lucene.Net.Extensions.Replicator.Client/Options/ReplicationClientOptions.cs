using Lucene.Net.Replicator;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace Lucene.Net.Extensions.Replicator.Client.Options;

/// <summary>
/// Options for configuring a Lucene.NET replication client.
/// </summary>
public class ReplicationClientOptions
{
    /// <summary>
    /// The URL of the replication server to pull index updates from.
    /// <para>Required.</para>
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;

    /// <summary>
    /// The path where the replica index will be stored.
    /// For disk-based directories this should point to a valid folder.
    /// </summary>
    public string IndexPath { get; set; } = string.Empty;

    /// <summary>
    /// An optional path for temporary files used during replication.
    /// Only relevant for disk-based directories.
    /// </summary>
    public string TempPath { get; set; } = string.Empty;

    /// <summary>
    /// How often the client will attempt to pull updates from the replication server.
    /// Defaults to 10 seconds.
    /// </summary>
    public TimeSpan PullInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Optional factory to create a custom <see cref="LuceneDirectory"/> implementation
    /// for the replica index (e.g., <see cref="Lucene.Net.Store.FSDirectory"/>,
    /// <see cref="Lucene.Net.Store.RAMDirectory"/>, or a custom one).
    /// If not provided, defaults to <see cref="Lucene.Net.Store.FSDirectory"/> on <see cref="IndexPath"/>.
    /// </summary>
    public Func<string, LuceneDirectory>? DirectoryFactory { get; set; }

    /// <summary>
    /// Optional factory for creating a custom <see cref="IReplicationHandler"/>.
    /// Defaults to <see cref="IndexReplicationHandler"/>.
    /// </summary>
    public Func<LuceneDirectory, IReplicationHandler> ReplicationHandlerFactory { get; set; }
        = dir => new IndexReplicationHandler(dir, null);
}
