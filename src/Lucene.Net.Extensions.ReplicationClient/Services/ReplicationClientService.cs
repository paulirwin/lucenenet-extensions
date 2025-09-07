using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Lucene.Net.Store;
using Lucene.Net.Replicator;
using Lucene.Net.Replicator.Http;
using Lucene.Net.Index;
using Lucene.Net.Extensions.ReplicationClient.Options;
using System.Diagnostics;
using System.IO;

namespace Lucene.Net.Extensions.ReplicationClient.Services;

public class ReplicationClientService : BackgroundService
{
    private readonly ILogger<ReplicationClientService> _logger;
    private readonly ReplicationClientOptions _options;
    private readonly HttpClient _httpClient;
    private Lucene.Net.Store.Directory? _replicaDirectory;
    private DirectoryReader? _reader;

    public ReplicationClientService(
    ILogger<ReplicationClientService> logger,
    IOptions<ReplicationClientOptions> options,
    IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _options = options.Value;
        _httpClient = httpClientFactory.CreateClient();

        // Validate critical options
        if (string.IsNullOrWhiteSpace(_options.ServerUrl))
            throw new InvalidOperationException("ServerUrl must be provided for the replication client.");

        if (_options.PullInterval <= TimeSpan.Zero)
        {
            _logger.LogWarning("PullInterval <= 0, defaulting to 10s");
            _options.PullInterval = TimeSpan.FromSeconds(10);
        }

        // Ensure TempPath exists if provided
        if (!string.IsNullOrWhiteSpace(_options.TempPath))
        {
            System.IO.Directory.CreateDirectory(_options.TempPath); // safe even if exists
            _logger.LogInformation("Using TempPath: {TempPath}", _options.TempPath);
        }

        // Warn if IndexPath does not exist (FSDirectory will create on first write)
        if (!string.IsNullOrWhiteSpace(_options.IndexPath) && !System.IO.Directory.Exists(_options.IndexPath))
        {
            _logger.LogWarning("IndexPath '{IndexPath}' does not exist. FSDirectory will create it on first write.",
                _options.IndexPath);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Use pluggable directory factory if provided, otherwise default FSDirectory
        _replicaDirectory = _options.DirectoryFactory?.Invoke(_options.IndexPath)
                            ?? FSDirectory.Open(_options.IndexPath);

        // Use TempPath only if directory is disk-based
        var factory = new PerSessionDirectoryFactory(
            IsDiskBasedDirectory(_replicaDirectory)
                ? _options.TempPath ?? Path.GetTempPath()
                : Path.GetTempPath() // dummy for RAMDirectory
        );

        var handler = new IndexReplicationHandler(_replicaDirectory, null);
        var replicator = new HttpReplicator(_options.ServerUrl, _httpClient);
        var client = new Lucene.Net.Replicator.ReplicationClient(replicator, handler, factory);

        _logger.LogInformation("Starting replication client from {ServerUrl} to {IndexPath}",
            _options.ServerUrl, _options.IndexPath);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                // Run replication in background with cancellation support
                await Task.Run(() => client.UpdateNow(), stoppingToken);

                sw.Stop();
                _logger.LogInformation("Replication successful from {ServerUrl} in {Duration} ms",
                    _options.ServerUrl, sw.ElapsedMilliseconds);

                // Efficient DirectoryReader update
                if (_reader == null)
                {
                    _reader = DirectoryReader.Open(_replicaDirectory);
                }
                else
                {
                    var newReader = DirectoryReader.OpenIfChanged(_reader);
                    if (newReader != null)
                    {
                        _reader.Dispose();
                        _reader = newReader;
                        _logger.LogInformation("DirectoryReader updated with new segment changes.");
                    }
                }

                _logger.LogInformation("Index at {IndexPath} now has {DocCount} docs",
                    _options.IndexPath, _reader.NumDocs);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Replication failed due to network issue. Retrying in {PullInterval}s",
                    _options.PullInterval.TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Replication failed");
            }

            await Task.Delay(_options.PullInterval, stoppingToken);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _reader?.Dispose();
        _replicaDirectory?.Dispose();
    }

    // Helper: check if directory is disk-based
    private static bool IsDiskBasedDirectory(Lucene.Net.Store.Directory directory) =>
        directory is FSDirectory or MMapDirectory or SimpleFSDirectory;
}
