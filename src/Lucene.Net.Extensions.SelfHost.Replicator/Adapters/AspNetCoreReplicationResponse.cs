using Lucene.Net.Replicator.Http.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lucene.Net.Extensions.SelfHost.Replicator.Adapters;

public class AspNetCoreReplicationResponse : IReplicationResponse
{
    private readonly HttpResponse _response;

    public AspNetCoreReplicationResponse(HttpResponse response)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public Stream OutputStream => _response.Body;

    public Stream Body => _response.Body;

    public int StatusCode
    {
        get => _response.StatusCode;
        set => _response.StatusCode = value;
    }

    public void SetStatusCode(int code)
    {
        _response.StatusCode = code;
    }

    public void SetHeader(string name, string value)
    {
        if (!string.IsNullOrWhiteSpace(name) && value != null)
        {
            _response.Headers[name] = value;
        }
    }

    public void Flush()
    {
        if (!_response.Body.CanWrite) return;

        // ASP.NET Core may throw InvalidOperationException if sync IO is not allowed
        try
        {
            _response.Body.FlushAsync().GetAwaiter().GetResult();
        }
        catch (InvalidOperationException)
        {
            // If async flush fails due to disallowed sync IO, attempt direct sync
            _response.Body.Flush();
        }
    }

    public async Task FlushAsync()
    {
        if (_response.Body.CanWrite)
        {
            await _response.Body.FlushAsync();
        }
    }
}

