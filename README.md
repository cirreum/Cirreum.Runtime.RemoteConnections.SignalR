# Cirreum.Runtime.RemoteConnections.SignalR

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.RemoteConnections.SignalR.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.RemoteConnections.SignalR/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.RemoteConnections.SignalR.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.RemoteConnections.SignalR/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.RemoteConnections.SignalR?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.RemoteConnections.SignalR/releases)
[![License](https://img.shields.io/badge/license-MIT-F2F2F2?style=flat-square&labelColor=1F1F1F)](https://github.com/cirreum/Cirreum.Runtime.RemoteConnections.SignalR/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**App-facing registration for Cirreum SignalR remote connections**

## Overview

**Cirreum.Runtime.RemoteConnections.SignalR** registers typed SignalR client connections on a Cirreum
application builder, replacing the hand-written singleton factory an application would otherwise
compose. Registration gives the connection framework-owned lifetime, reconnect policy, credential
refresh, observable state, and disposal.

The transport implementation ships in `Cirreum.RemoteConnections.SignalR` and flows in transitively.

## Usage

Write the connection type. It derives from `SignalRRemoteConnection` and takes the
framework-supplied context as its first constructor parameter; anything else resolves from the
container as usual:

```csharp
public sealed class ChatConnection(SignalRRemoteConnectionContext context)
    : SignalRRemoteConnection(context) {

    public IDisposable OnMessage(Func<ChatMessage, Task> handler) =>
        this.On("ReceiveMessage", handler);

    public Task SendMessageAsync(ChatMessage message, CancellationToken ct = default) =>
        this.SendAsync("SendMessage", message, ct);

}
```

Register it on the builder:

```csharp
var builder = DomainApplication.CreateBuilder(args);

builder.AddRemoteConnection<ChatConnection>(options => {
    options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
});
```

The connection resolves as `ChatConnection` and as `IRemoteConnection`, so a status surface can
inject `IEnumerable<IRemoteConnection>` and render every connection's state without knowing the
concrete types.

Registration does not connect. Inject the connection and connect when the caller is ready —
typically after sign-in rather than at startup:

```csharp
protected override async Task OnInitializedAsync() {
    this._subscription = this.Chat.OnMessage(this.HandleMessageAsync);
    this.Chat.StateChanged += this.OnConnectionStateChanged;
    await this.Chat.ConnectAsync();
}
```

Never dispose an injected connection: the container created it and disposes it with the host.

### Per-session connections

A connection that belongs to a session rather than to the application — one per phone call, one per
bridge — registers a factory instead:

```csharp
builder.AddRemoteConnectionFactory<TranscriptionConnection>(options => {
    options.EndpointUri = new Uri("https://api.example.com/hubs/transcription");
});
```

This registers `IRemoteConnectionFactory<TranscriptionConnection>` and no connection instance, so a
status surface enumerating standing connections never sees per-session ones. Ownership inverts with
the lifetime: the caller creates, connects, and disposes what the factory returns.

```csharp
await using var session = this._transcriptionFactory.Create();
await session.ConnectAsync(ct);
```

`Create` optionally adjusts the registered options for one session, leaving the registration
untouched for every later one.

### Notes

- **One registration per connection type**, in either shape. Registering the same type twice with
  equal options is a no-op; with different options, or under both verbs, it throws. Subclass the
  connection to reach a second endpoint.
- **Credentials** resolve from the options when set, and otherwise from the host's ambient
  `IRemoteConnectionCredentialSource`, which the host runtime registers. Name the audience on the
  options and the host mints for it:

  ```csharp
  builder.AddRemoteConnection<ChatConnection>(options => {
      options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
      options.Scopes = ["api://contoso/access_as_user"];
  });
  ```

  A source registered *keyed* to a connection type is preferred over the unkeyed one for that
  connection, so one connection can use a different mechanism or identity provider than another:

  ```csharp
  services.AddKeyedScoped<IRemoteConnectionCredentialSource, PartnerCredentialSource>(typeof(PartnerConnection));
  ```

  For a factory registration the options are copied per instance, so `Create`'s adjustment — a
  different audience for one session — does not reach the registration every later session is built
  from.
- **The native transport** is reachable through the optional `configureTransport` delegate, which
  receives the `IHubConnectionBuilder` itself after the framework has configured it.

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