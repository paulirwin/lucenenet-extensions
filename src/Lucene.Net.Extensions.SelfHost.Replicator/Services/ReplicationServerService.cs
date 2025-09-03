using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Lucene.Net.Replicator;
using Lucene.Net.Replicator.Http;
using Microsoft.Extensions.DependencyInjection;
using Lucene.Net.Extensions.SelfHost.Replicator.Options;
using Lucene.Net.Extensions.AspNetCore.Replicator.Adapters;

namespace Lucene.Net.Extensions.SelfHost.Replicator.Services;

/// <summary>
/// A background service that hosts a self-contained Lucene.NET replication server using Kestrel.
/// </summary>
public class ReplicationServerService : BackgroundService
{
    private readonly ILogger<ReplicationServerService> _logger;
    private readonly ReplicationServerOptions _options;
    private ReplicationService? _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplicationServerService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging server activity and errors.</param>
    /// <param name="options">The options for configuring the replication server.</param>
    public ReplicationServerService(
        ILogger<ReplicationServerService> logger,
        IOptions<ReplicationServerOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    /// <summary>
    /// Executes the replication server as a background task.
    /// </summary>
    /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the lifetime of the replication server.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Starting Lucene Replication Server on port {Port} with {ShardCount} shard(s)...",
            _options.Port, _options.Replicators.Count);

        // Build one ReplicationService for all shards
        _service = new ReplicationService(_options.Replicators);

        var app = SetupKestrelServer();
        app.Urls.Add($"http://localhost:{_options.Port}");
        await app.RunAsync(stoppingToken);
    }

    /// <summary>
    /// Configures and builds a Kestrel-based <see cref="WebApplication"/> for handling replication requests.
    /// </summary>
    /// <returns>The configured <see cref="WebApplication"/> instance.</returns>
    private WebApplication SetupKestrelServer()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.Configure<KestrelServerOptions>(o => o.AllowSynchronousIO = true);

        var app = builder.Build();

        app.Map("/replicate/{shard}/{action}", async (HttpContext context, string shard, string action) =>
        {
            if (!_options.Replicators.ContainsKey(shard))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Unknown shard: {shard}");
                return;
            }

            try
            {
                var req = new AspNetCoreReplicationRequest(context.Request);
                var res = new AspNetCoreReplicationResponse(context.Response);

                // Reuse the single ReplicationService instance
                // TODO: Make this async once Lucene.NET PR #1170 is merged and released
                _service!.Perform(req, res);

                await res.FlushAsync(context.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling replication request for shard {Shard}", shard);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        });

        return app;
    }
}
