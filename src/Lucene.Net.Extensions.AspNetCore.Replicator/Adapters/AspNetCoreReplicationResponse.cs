using Lucene.Net.Replicator.Http.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lucene.Net.Extensions.AspNetCore.Replicator.Adapters;

/// <summary>
/// Adapter that wraps an ASP.NET Core <see cref="HttpResponse"/> and exposes it
/// as an <see cref="IReplicationResponse"/> for Lucene.NET replication.
/// </summary>
public class AspNetCoreReplicationResponse : IReplicationResponse
{
    private readonly HttpResponse _response;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetCoreReplicationResponse"/> class.
    /// </summary>
    /// <param name="response">The ASP.NET Core <see cref="HttpResponse"/> to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="response"/> is null.</exception>
    public AspNetCoreReplicationResponse(HttpResponse response)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
    }

    /// <summary>
    /// Gets the output stream of the HTTP response.
    /// </summary>
    public Stream OutputStream => _response.Body;

    /// <summary>
    /// Gets the body stream of the HTTP response.
    /// </summary>
    public Stream Body => _response.Body;

    /// <summary>
    /// Gets or sets the HTTP status code of the response.
    /// </summary>
    public int StatusCode
    {
        get => _response.StatusCode;
        set => _response.StatusCode = value;
    }

    /// <summary>
    /// Sets the HTTP status code of the response.
    /// </summary>
    /// <param name="code">The HTTP status code to set.</param>
    public void SetStatusCode(int code)
    {
        _response.StatusCode = code;
    }

    /// <summary>
    /// Sets a header on the HTTP response.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value.</param>
    public void SetHeader(string name, string value)
    {
        if (!string.IsNullOrWhiteSpace(name) && value != null)
        {
            _response.Headers[name] = value;
        }
    }

    /// <summary>
    /// Flushes the response body synchronously.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if synchronous flush is not allowed (e.g., when AllowSynchronousIO = false).
    /// </exception>
    public void Flush()
    {
        if (!_response.Body.CanWrite)
            return;

        try
        {
            _response.Body.Flush();
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                "Synchronous Flush is not supported when AllowSynchronousIO = false. " +
                "Use FlushAsync() instead, or enable AllowSynchronousIO if you must call Flush().", ex);
        }
    }

    /// <summary>
    /// Asynchronously flushes the response body.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the flush to complete.
    /// </param>
    /// <returns>A task representing the asynchronous flush operation.</returns>
    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        if (_response.Body.CanWrite)
        {
            await _response.Body.FlushAsync(cancellationToken);
        }
    }
}

