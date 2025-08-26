using Lucene.Net.Replicator;

namespace Lucene.Net.Extensions.SelfHost.Replicator.Options;

/// <summary>
/// Options for configuring a Lucene.NET self-hosted replication server.
/// </summary>
public class ReplicationServerOptions
{
    /// <summary>
    /// Gets or sets the port number on which the replication server will listen.
    /// Defaults to 5000.
    /// </summary>
    public int Port { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the collection of shard names mapped to their <see cref="IReplicator"/> instances.
    /// User must supply ready-to-use replicators.
    /// </summary>
    public Dictionary<string, IReplicator> Replicators { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);
}
