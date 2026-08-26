# Cirreum.Runtime.RemoteConnections.SignalR 2.0.0 — the registration names the connection

## Why this release exists

The credential seam beneath this package changed. A credential source used to take no parameters, so
one registration answered identically for every connection an application opened — and where the
host could not infer an audience it supplied its own defaults, which on WebAssembly are Microsoft
Graph scopes. The framework's out-of-box credential could not authenticate a hub on the
application's own backend.

The fix is that a source is told which connection it is supplying for. This package is where that
information exists: it holds `TConnection`, and the transport beneath it does not.

## What's new

**A connection names its audience:**

```csharp
builder.AddRemoteConnection<ChatConnection>(options => {
    options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
    options.Scopes = ["api://contoso/access_as_user"];
});
```

The scopes reach the host's credential source, which mints for them rather than for its own
defaults. No per-application source to write.

**The registration stamps the connection type**, and a source registered keyed to that type is
preferred over the unkeyed one:

```csharp
services.AddKeyedScoped<IRemoteConnectionCredentialSource, PartnerCredentialSource>(typeof(PartnerConnection));
```

So a connection to a partner service can use a different mechanism, or a different identity
provider, than one to your own API — without either connection knowing about the other.

## Fixed

**A factory-created connection carries the registered scopes.** The factory copies the registered
options per instance, and that copy is the only path a per-session connection's options travel. It
omitted `Scopes`, which would have left every per-session connection's credential source with no
audience to mint for — silently, since a forgotten property looks exactly like one that was never
set. Caught by a test written for the copy rather than for the feature.

## Compatibility

- **The registration verbs are unchanged**, including their overloads, the
  one-registration-per-connection-type rule, registration-time validation, and options-equality
  dedup.
- **One `using` changes** where an application writes a connection type: the connection types moved
  to `Cirreum.RemoteServices.Connections`.
- **Declare `Scopes` on every connection before upgrading.** Paired with `Cirreum.Runtime.Wasm`
  4.0.0, a connection that declares none receives no credential and fails at connect. See
  [MIGRATION-v2.md](MIGRATION-v2.md).

## See also

- `Cirreum.RemoteConnections.SignalR` 2.0.0 — the transport that resolves the source.
- `Cirreum.Contracts` 5.0.0 — the credential contract and the reasoning behind its shape.
- `Cirreum.Runtime.Wasm` 4.0.0 — the browser credential source that mints for the declared scopes.
