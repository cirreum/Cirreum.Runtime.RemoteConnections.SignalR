# Cirreum.Runtime.RemoteConnections.SignalR 1.0.0

First release of the app-facing registration surface for Cirreum SignalR remote connections.

One builder line registers a typed connection with framework-owned lifetime, reconnect, token refresh, and state. `AddRemoteConnection` serves app-lifetime connections; `AddRemoteConnectionFactory` serves per-session connections such as one per call or per bridge.
