using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Lucene.Net.Replicator;
using Lucene.Net.Extensions.SelfHost.Replicator.Options;
using Lucene.Net.Extensions.SelfHost.Replicator.Services;

namespace Lucene.Net.Extensions.SelfHost.Replicator;

/// <summary>
/// Provides extension methods for registering a Lucene.NET replication server in an ASP.NET Core <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures a self-hosted Lucene.NET replication server as a hosted service.
    /// </summary>
    /// <param name="services">The service collection to which the replication server will be added.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="ReplicationServerOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddLuceneReplicationServer(
        this IServiceCollection services,
        Action<ReplicationServerOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHostedService<ReplicationServerService>();

        return services;
    }
}
