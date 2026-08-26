namespace Cirreum.Runtime;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Creates per-session <typeparamref name="TConnection"/> instances from the registered options.
/// </summary>
internal sealed class SignalRRemoteConnectionFactory<TConnection>(
	IServiceProvider services,
	RemoteConnectionOptions options,
	Action<IHubConnectionBuilder>? configureTransport)
	: IRemoteConnectionFactory<TConnection>
	where TConnection : SignalRRemoteConnection {

	public TConnection Create(Action<RemoteConnectionOptions>? configure = null) {

		// Each instance configures a copy: an adjustment made for one session must not reach
		// the registration every later session builds from.
		var instanceOptions = Copy(options);
		configure?.Invoke(instanceOptions);

		var context = SignalRRemoteConnectionContext.Create<TConnection>(services, instanceOptions, configureTransport);
		return ActivatorUtilities.CreateInstance<TConnection>(services, context);

	}

	private static RemoteConnectionOptions Copy(RemoteConnectionOptions source) {
		return new RemoteConnectionOptions(source.ApplicationName) {
			EndpointUri = source.EndpointUri,
			AuthorizationHeader = source.AuthorizationHeader,
			CredentialProvider = source.CredentialProvider,
			Scopes = source.Scopes,
			Reconnect = source.Reconnect,
			ReconnectMaxDelay = source.ReconnectMaxDelay,
		};
	}

}
