using System;
using Microsoft.Extensions.DependencyInjection;
using Lucene.Net.Extensions.ReplicationClient.Options;
using Lucene.Net.Extensions.ReplicationClient.Services;

namespace Lucene.Net.Extensions.ReplicationClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuceneReplicationClient(this IServiceCollection services, Action<ReplicationClientOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddHostedService<ReplicationClientService>();
        return services;
    }
}
