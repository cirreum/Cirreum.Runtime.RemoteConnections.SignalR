# Migration to v1

Initial release — there is no prior version of **Cirreum.Runtime.RemoteConnections.SignalR** to migrate from.

Applications previously wiring `HubConnection` by hand in `Program.cs` replace that wiring with a single builder call. Connection types derive from `SignalRRemoteConnection`; handler registrations and typed hub methods move onto the derived class.
