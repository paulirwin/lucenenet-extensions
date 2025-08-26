using Lucene.Net.Replicator.Http.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;

namespace Lucene.Net.Extensions.AspNetCore.Replicator.Adapters;

/// <summary>
/// Adapter that wraps an ASP.NET Core <see cref="HttpRequest"/> and exposes it
/// as a <see cref="IReplicationRequest"/> for Lucene.NET replication.
/// </summary>
public class AspNetCoreReplicationRequest : IReplicationRequest
{
    private readonly HttpRequest _request;

    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetCoreReplicationRequest"/> class.
    /// </summary>
    /// <param name="request">The ASP.NET Core <see cref="HttpRequest"/> to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
    public AspNetCoreReplicationRequest(HttpRequest request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }

    /// <summary>
    /// Gets the body of the HTTP request as a <see cref="Stream"/>.
    /// </summary>
    public Stream InputStream => _request.Body;

    /// <summary>
    /// Gets the HTTP method of the request (e.g., GET, POST).
    /// </summary>
    public string Method => _request.Method;

    /// <summary>
    /// Gets the path of the HTTP request.
    /// </summary>
    public string Path => _request.Path;

    /// <summary>
    /// Gets a query parameter or route value by name.
    /// </summary>
    /// <param name="name">The name of the query parameter or route value.</param>
    /// <returns>The value as a string if found; otherwise, <c>null</c>.</returns>
    public string? QueryParam(string name)
    {
        if (_request.Query.TryGetValue(name, out var queryVal))
            return queryVal.ToString();

        var routeData = _request.HttpContext.GetRouteData();
        if (routeData.Values.TryGetValue(name, out var routeVal))
            return routeVal?.ToString();

        return null;
    }

    /// <summary>
    /// Gets the value of a specific HTTP header.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <returns>The header value if present; otherwise, <c>null</c>.</returns>
    public string? GetHeader(string name)
    {
        return _request.Headers[name].FirstOrDefault();
    }
}
