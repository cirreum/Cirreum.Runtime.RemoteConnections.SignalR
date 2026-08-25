# Cirreum.Runtime.RemoteConnections.SignalR 1.0.0

First release of the app-facing registration surface for Cirreum SignalR remote connections.

## What this is for

`Cirreum.RemoteConnections.SignalR` supplies the connection; this package supplies the line that
registers it. Without it an application composes the singleton itself — build the context from the
provider, construct the connection, forward it as `IRemoteConnection`, and remember to do all three
the same way for the next connection. That composition is identical in every application, which
makes it exactly the kind of thing that drifts.

## What it provides

**`AddRemoteConnection<TConnection>()`** registers a connection that lives as long as the
application:

```csharp
builder.AddRemoteConnection<ChatConnection>(options => {
    options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
});
```

It resolves as `ChatConnection` and as `IRemoteConnection`, so a status surface can inject
`IEnumerable<IRemoteConnection>` and render every connection's state without knowing the concrete
types. The container disposes it with the host; an application never disposes an injected
connection.

Registration does not connect. The application connects when the caller is ready — typically after
sign-in rather than at startup.

**`AddRemoteConnectionFactory<TConnection>()`** serves the other lifetime: a connection belonging to
a session rather than to the application, one per call or per bridge. It registers
`IRemoteConnectionFactory<TConnection>` and no connection instance, so a status surface enumerating
standing connections never sees per-session ones. Ownership inverts with the lifetime — the caller
creates, connects, and disposes what the factory returns.

## Registration rules

**One registration per connection type**, in either shape. Registering the same type twice with
equal options is a no-op; with different options, or under both verbs, it throws. Subclass the
connection to reach a second endpoint — the client-side analogue of the server's one-instance-per-hub-type
rule.

The registry is keyed by service collection rather than held process-wide. A registry shared across
a process would make a second container — a test host, a second builder — silently skip registrations
the first container had already claimed.

**Options are validated as they are registered.** A missing or relative endpoint surfaces while the
application is composing, not when something first resolves the connection.

## Host neutrality

Both verbs are extension members on `IDomainApplicationBuilder`, which a Blazor WebAssembly builder
and a server-side builder both implement. There is no per-host variant, and no `.Wasm` package.

## Requirements

* `Cirreum.RemoteConnections.SignalR` 1.0.0 or later, which carries the transport and
  `SignalRRemoteConnection`. It flows in transitively, along with `Cirreum.Domain`,
  `Cirreum.Contracts` and `Cirreum.Kernel`.
* A host registering `IRemoteConnectionTokenSource` if connections are to present the session
  credential without configuring one — `Cirreum.Runtime.Wasm` 3.0.0 does.
