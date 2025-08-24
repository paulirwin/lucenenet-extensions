using Lucene.Net.Replicator.Http.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;

namespace Lucene.Net.Extensions.SelfHost.Replicator.Adapters;

public class AspNetCoreReplicationRequest : IReplicationRequest
{
    private readonly HttpRequest _request;

    public AspNetCoreReplicationRequest(HttpRequest request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Stream InputStream => _request.Body;

    public string Method => _request.Method;

    public string Path => _request.Path;

    public string? QueryParam(string name)
    {
        if (_request.Query.TryGetValue(name, out var queryVal))
            return queryVal.ToString();

        var routeData = _request.HttpContext.GetRouteData();
        if (routeData.Values.TryGetValue(name, out var routeVal))
            return routeVal?.ToString();

        return null;
    }

    public string? GetHeader(string name)
    {
        return _request.Headers[name].FirstOrDefault();
    }
}

