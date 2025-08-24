using Lucene.Net.Replicator;

namespace Lucene.Net.Extensions.SelfHost.Replicator.Options;

public class ReplicationServerOptions
{
    public int Port { get; set; } = 5000;
    // User supplies ready-to-use replicators
    public Dictionary<string, IReplicator> Replicators { get; set; }
        = new(StringComparer.OrdinalIgnoreCase);
}
