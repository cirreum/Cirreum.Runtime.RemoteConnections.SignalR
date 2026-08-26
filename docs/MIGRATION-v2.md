# Cirreum.Runtime.RemoteConnections.SignalR v1 → v2 Migration

v2 follows `Cirreum.Contracts` 5.0.0, `Cirreum.Domain` 5.0.0 and
`Cirreum.RemoteConnections.SignalR` 2.0.0. The registration verbs are unchanged; what moves is
underneath them.

---

## 1. Namespace

| v1 | v2 |
| --- | --- |
| `using Cirreum.RemoteServices;` | `using Cirreum.RemoteServices.Connections;` |

The connection types moved. `AuthorizationHeaderSettings` and `RemoteIdentityConstants` did not — a
file touching both imports both namespaces.

A remote service is something you *call*; a remote connection is something you *hold open*. The
second is a relationship with a remote service rather than a peer of one, so it nests.

`AddRemoteConnection<TConnection>` and `AddRemoteConnectionFactory<TConnection>` are extension
members on `IDomainApplicationBuilder` in `Cirreum.Runtime`, as before.

## 2. The credential seam

| v1 | v2 |
| --- | --- |
| `options.AccessTokenProvider` | `options.CredentialProvider` |
| `Func<CancellationToken, ValueTask<string?>>` | `Func<CancellationToken, ValueTask<AuthorizationHeaderSettings?>>` |
| `IRemoteConnectionTokenSource` | `IRemoteConnectionCredentialSource` |

The full before/after is in `Cirreum.Contracts`' `MIGRATION-v5.md` — one guide for the whole track.

## 3. ⚠️ Declare each connection's audience

**Behavioural, not a compile error.**

A connection now names the audience its credential is minted for:

```csharp
builder.AddRemoteConnection<ChatConnection>(options => {
    options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
    options.Scopes = ["api://contoso/access_as_user"];
});
```

Paired with `Cirreum.Runtime.Wasm` 4.0.0, a connection that declares **no** scopes receives no
credential and fails at connect with a warning naming it. In v1 it received a token for the host's
default scopes — on Entra, Microsoft Graph — which no first-party API accepts, and the rejection
arrived at the server reading as an application authentication fault.

Add `Scopes` to every connection before upgrading. `Cirreum.Runtime.Wasm`'s `MIGRATION-v4.md` has
the deploy order.

## New capabilities

### A credential source per connection

```csharp
services.AddKeyedScoped<IRemoteConnectionCredentialSource, PartnerCredentialSource>(typeof(PartnerConnection));
```

The registration stamps the connection type into the credential request, and a source registered
keyed to that type is preferred over the unkeyed one — so a connection to a partner service can use
a different mechanism or identity provider than one to your own API.

## Fixed

A factory-created connection now carries the registered `Scopes`. The per-instance options copy is
the only path a per-session connection's options travel; a property it omits is lost in silence.

## What didn't change

- Both registration verbs, their overloads, and the one-registration-per-connection-type rule.
- Registration-time options validation, and options-equality dedup.
- `IRemoteConnection` forwarding for `AddRemoteConnection`, and its deliberate absence for
  `AddRemoteConnectionFactory`.
- The `configureTransport` escape hatch.
