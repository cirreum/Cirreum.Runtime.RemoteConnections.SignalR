# Changelog

All notable changes to **Cirreum.Runtime.RemoteConnections.SignalR** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For detailed migration steps on major version bumps, see the per-version migration
guides linked at the bottom of each entry.

---

## [Unreleased]

## [2.0.0] - 2026-08-26

### Breaking

* **The connection types move to `Cirreum.RemoteServices.Connections`**, following
  `Cirreum.Contracts` 5.0.0, `Cirreum.Domain` 5.0.0 and `Cirreum.RemoteConnections.SignalR` 2.0.0.
  A service is something you call; a connection is something you hold open, so it nests rather than
  sitting alongside. An application writing a connection type changes one `using`.

* **The credential seam follows the transport.** `RemoteConnectionOptions.CredentialProvider`
  replaces `AccessTokenProvider`, and the ambient source is `IRemoteConnectionCredentialSource`.
  A resolved credential that is `null` now fails the connect rather than opening an unauthenticated
  one; `AuthorizationHeaderSettings.None` is how a connection says it wants none.

### Added

* **`RemoteConnectionOptions.Scopes` is carried through registration**, so a connection names the
  audience its credential is minted for and the host runtime needs no per-application source.

* **The registration stamps the connection type**, which reaches the credential source in its
  request. A source registered keyed to that type is preferred over the unkeyed one, so one
  connection can use a different mechanism or identity provider than another.

### Fixed

* **A factory-created connection carries the registered scopes.** The per-instance options copy is
  the only path a per-session connection's options travel, and a property it omits is lost in
  silence — for scopes, that is a connection whose credential source is never told which audience
  to mint for.

### Updated

- `Cirreum.RemoteConnections.SignalR` 2.0.0.

### Updated

- Updated NuGet packages.

## [1.0.0] - 2026-08-25

Initial release of **Cirreum.Runtime.RemoteConnections.SignalR**.

### Added

* **`AddRemoteConnection<TConnection>()`** — registers a typed SignalR connection living for the
  lifetime of the application. The connection resolves as `TConnection` and as `IRemoteConnection`,
  so a status surface injects `IEnumerable<IRemoteConnection>` and renders every connection's state
  without knowing the concrete types. The container disposes it with the host.
* **`AddRemoteConnectionFactory<TConnection>()`** — registers
  `IRemoteConnectionFactory<TConnection>` for connections belonging to a session rather than to the
  application: one per call, one per bridge. It registers no connection instance and no
  `IRemoteConnection` forwarding, so a status surface enumerating standing connections never sees
  per-session ones, and the caller owns what it creates. `Create` optionally adjusts the registered
  options for one session, leaving the registration untouched for every later one.
* **One registration per connection type**, in either shape. Registering the same type twice with
  equal options is a no-op; with different options, or under both verbs, it throws. Subclassing the
  connection reaches a second endpoint. The registry is keyed by service collection rather than held
  process-wide, so a second container in the same process — a test host, a second builder — composes
  its own registrations rather than silently skipping them.
* **Options are validated as they are registered**, so a missing or relative endpoint surfaces while
  the application composes rather than when something first resolves the connection.
* **`configureTransport`** exposes the native `IHubConnectionBuilder`, applied after the framework
  has configured it, so any transport setting can be overridden.

Both verbs are extension members on `IDomainApplicationBuilder`, so the package serves a Blazor
WebAssembly client and a server-side host without a per-host variant.
