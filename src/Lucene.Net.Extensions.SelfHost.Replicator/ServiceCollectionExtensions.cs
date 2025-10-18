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
