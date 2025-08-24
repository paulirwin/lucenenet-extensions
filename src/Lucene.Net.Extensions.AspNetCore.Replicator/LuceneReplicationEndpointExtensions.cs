using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lucene.Net.Replicator;
using Lucene.Net.Replicator.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Lucene.Net.Extensions.AspNetCore.Replicator.Adapters;

namespace Lucene.Net.Extensions.AspNetCore.Replicator;

public static class LuceneReplicationEndpointExtensions
{
    public static IEndpointRouteBuilder MapLuceneReplicationServer(
        this IEndpointRouteBuilder endpoints,
        string basePath,
        IDictionary<string, IReplicator> shardMap)
    {
        var contextPath = NormalizeContextPath(basePath);

        var replicationService = new ReplicationService(
            new Dictionary<string, IReplicator>(shardMap, StringComparer.OrdinalIgnoreCase),
            context: contextPath);

        var pattern = $"{contextPath}/{{shard}}/{{action}}";

        endpoints.Map(pattern, async context =>
        {
            try
            {
                var req = new AspNetCoreReplicationRequest(context.Request);
                var res = new AspNetCoreReplicationResponse(context.Response);

                replicationService.Perform(req, res);
                await res.FlushAsync();
            }
            catch (Exception ex)
            {
                var logger = context.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("LuceneReplication");
                logger?.LogError(ex, "Replication request failed.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Replication error.");
            }
        });

        return endpoints;
    }

    private static string NormalizeContextPath(string basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
            throw new ArgumentException("Base path cannot be null or empty.", nameof(basePath));

        return "/" + basePath.Trim().Trim('/');
    }
}
