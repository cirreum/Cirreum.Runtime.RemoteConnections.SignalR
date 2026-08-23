# Cirreum.Runtime.RemoteConnections.SignalR

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.RemoteConnections.SignalR.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.RemoteConnections.SignalR/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.RemoteConnections.SignalR.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.RemoteConnections.SignalR/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.RemoteConnections.SignalR?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.RemoteConnections.SignalR/releases)
[![License](https://img.shields.io/badge/license-MIT-F2F2F2?style=flat-square&labelColor=1F1F1F)](https://github.com/cirreum/Cirreum.Runtime.RemoteConnections.SignalR/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**App-facing registration for Cirreum SignalR remote connections**

## Overview

**Cirreum.Runtime.RemoteConnections.SignalR** registers typed SignalR client connections on a Cirreum application builder.

```csharp
builder.AddRemoteConnection<ChatConnection>(options => {
    options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
});
```

That one line gives the connection framework-owned lifetime, reconnect policy, access-token refresh,
observable state, and disposal. Use `AddRemoteConnectionFactory<TConnection>()` instead when a
connection belongs to a session rather than to the application — one per call, one per bridge.

The transport implementation ships in `Cirreum.RemoteConnections.SignalR` and flows in
transitively.

## Documentation

- [CHANGELOG](docs/CHANGELOG.md)
- [Backlog](docs/BACKLOG.md)

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.Runtime.RemoteConnections.SignalR follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*