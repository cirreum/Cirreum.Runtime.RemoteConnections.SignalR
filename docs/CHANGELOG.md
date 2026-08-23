# Changelog

All notable changes to **Cirreum.Runtime.RemoteConnections.SignalR** are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

For detailed migration steps on major version bumps, see the per-version migration
guides linked at the bottom of each entry.

---

## [Unreleased]

Initial release of **Cirreum.Runtime.RemoteConnections.SignalR**.

### Added

- `AddRemoteConnection<TConnection>()` — registers an app-lifetime typed SignalR connection as a singleton, forwarded as `IRemoteConnection` for status surfaces.
- `AddRemoteConnectionFactory<TConnection>()` — registers `IRemoteConnectionFactory<TConnection>` for per-session connections the caller creates, connects, and disposes.
- `RemoteConnectionOptions` binding, options validation at registration, and a `configureTransport` escape hatch exposing the native `IHubConnectionBuilder`.
