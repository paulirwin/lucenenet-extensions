using System;
using Microsoft.Extensions.DependencyInjection;
using Lucene.Net.Extensions.Replicator.Client.Options;
using Lucene.Net.Extensions.Replicator.Client.Services;

namespace Lucene.Net.Extensions.Replicator.Client;

/// <summary>
/// Provides extension methods for registering the Lucene.NET replication client
/// with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Registers a Lucene.NET replication client as a hosted background service.
    /// </summary>
    /// <param name="services">The service collection to add the client to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="ReplicationClientOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddLuceneReplicationClient(this IServiceCollection services, Action<ReplicationClientOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHostedService<ReplicationClientService>();
        return services;
    }
}
