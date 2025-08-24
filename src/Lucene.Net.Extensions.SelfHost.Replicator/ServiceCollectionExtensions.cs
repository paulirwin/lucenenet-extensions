using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Lucene.Net.Replicator;
using Lucene.Net.Extensions.SelfHost.Replicator.Options;
using Lucene.Net.Extensions.SelfHost.Replicator.Services;

namespace Lucene.Net.Extensions.SelfHost.Replicator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuceneReplicationServer(this IServiceCollection services, Action<ReplicationServerOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHostedService<ReplicationServerService>();

        return services;
    }
}
