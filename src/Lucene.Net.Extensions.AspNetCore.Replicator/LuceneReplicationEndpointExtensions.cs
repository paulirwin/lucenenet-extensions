/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lucene.Net.Replicator;
using Lucene.Net.Replicator.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Lucene.Net.Extensions.AspNetCore.Replicator.Adapters;

namespace Lucene.Net.Extensions.AspNetCore.Replicator;

/// <summary>
/// Provides extension methods to map Lucene.NET replication endpoints to ASP.NET Core routing.
/// </summary>
public static class LuceneReplicationEndpointExtensions
{
    /// <summary>
    /// Maps a Lucene.NET replication server to an ASP.NET Core endpoint route.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to which the replication endpoint will be added.</param>
    /// <param name="basePath">The base path for the replication endpoint (e.g., "/lucene").</param>
    /// <param name="shardMap">A dictionary mapping shard names to their <see cref="IReplicator"/> instances.</param>
    /// <returns>The original <see cref="IEndpointRouteBuilder"/> with the replication endpoint mapped.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="basePath"/> is null or empty.</exception>
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

                // TODO: Make this async once Lucene.NET PR #1170 is merged and released
                replicationService.Perform(req, res);

                await res.FlushAsync(context.RequestAborted);
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
        {
            throw new ArgumentException("Base path cannot be null or empty.", nameof(basePath));
        }

        return "/" + basePath.Trim().Trim('/');
    }
}
