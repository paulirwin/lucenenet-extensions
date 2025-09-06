# Apache Lucene.NET Extensions

Lucene.NET Extensions is a collection of libraries that provide additional functionality for [Apache Lucene.NET](https://github.com/apache/lucenenet).

All features are open to contribution by the community!

> [!WARNING]
> The packages in this repo have not yet been released to NuGet. Currently, you must build them manually if you want to use them. This will be fixed soon!

## What does this repo contain?

| Package | Description |
| --- | --- |
| `Lucene.Net.Extensions.DependencyInjection` | Extension methods to make it easier to configure Apache Lucene.NET with [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (aka any modern .NET application) |
| `Lucene.Net.Extensions.AspNetCore.Replicator` | Replicator support for ASP.NET Core apps |
| `Lucene.Net.Extensions.SelfHost.Replicator` | Replicator support as a self-hosted Background Service (i.e. for Windows/Linux services) |

## Supported Frameworks

Lucene.NET Extensions will only support the versions of modern .NET that are under active support according to the [.NET official support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core#lifecycle). There is no intention to support .NET Framework for these libraries.

Currently, this means the following frameworks are supported:
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)

This repo will more closely align its releases based on .NET lifecycle than Apache Lucene.NET releases.

## Documentation

> [!WARNING]
> This project does not yet have published documentation. We welcome any contributions to help with this!

## How to Contribute

We love getting contributions! Read the main [Apache Lucene.NET Contribution Guide](https://github.com/apache/lucenenet/blob/master/CONTRIBUTING.md) or the [Apache Lucene.NET README](https://github.com/apache/lucenenet/tree/master?tab=readme-ov-file#how-to-contribute) for ways you can help.

## Building and Testing

### Common Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/visual-studio-sdks)

### Command Line

These commands work the same on Windows, macOS, and Linux.

#### Build

From the repo root:
```bash
dotnet build
```

#### Run Unit Tests

From the repo root:
```bash
dotnet test
```

### IDE (Visual Studio 2022, Visual Studio Code, JetBrains Rider)

#### Prerequisites

Choose one:
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (Windows only)
  - The [Community Edition](https://visualstudio.microsoft.com/vs/community/) is free for open-source contributions to our project! 
- [Visual Studio Code](https://code.visualstudio.com/) with [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) installed
  - NOTE: C# Dev Kit is only required to open the solution and run unit tests in the IDE. You can still use Visual Studio Code as an editor with the `dotnet` CLI commands above. C# Dev Kit is not freely available without a compatible subscription.
- [JetBrains Rider](https://www.jetbrains.com/rider/)
  - Rider is free for non-commercial use, like open-source contributions to our project!

#### Build and Test

1. Open `Lucene.Net.Extensions.sln` in your IDE.
2. Build the solution. You might get build failures if you are missing the SDKs above.
3. Run all unit tests in the solution.
