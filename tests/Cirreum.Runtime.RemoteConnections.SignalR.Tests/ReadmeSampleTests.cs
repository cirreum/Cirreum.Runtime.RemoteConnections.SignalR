namespace Cirreum.Runtime.RemoteConnections.SignalR.Tests;

using Cirreum;
using Cirreum.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Compiles the connection type and the registration the README documents. A sample that no
/// longer matches the surface fails the build here rather than at a reader.
/// </summary>
public class ReadmeSampleTests {

	public sealed record ChatMessage(string Room, string Text);

	// README — "Write the connection type"
	public sealed class ChatConnection(SignalRRemoteConnectionContext context)
		: SignalRRemoteConnection(context) {

		public IDisposable OnMessage(Func<ChatMessage, Task> handler) =>
			this.On("ReceiveMessage", handler);

		public Task SendMessageAsync(ChatMessage message, CancellationToken ct = default) =>
			this.SendAsync("SendMessage", message, ct);

	}

	// README — "Per-session connections"
	public sealed class TranscriptionConnection(SignalRRemoteConnectionContext context)
		: SignalRRemoteConnection(context);

	private sealed class SampleBuilder : IDomainApplicationBuilder {

		public IServiceCollection Services { get; } = new ServiceCollection();

		public ILoggingBuilder Logging => throw new NotSupportedException();

	}

	[Fact]
	public void The_documented_registrations_compile_and_resolve() {

		var builder = new SampleBuilder();

		// README — "Register it on the builder"
		builder.AddRemoteConnection<ChatConnection>(options => {
			options.EndpointUri = new Uri("https://api.example.com/hubs/chat");
		});

		// README — "Per-session connections"
		builder.AddRemoteConnectionFactory<TranscriptionConnection>(options => {
			options.EndpointUri = new Uri("https://api.example.com/hubs/transcription");
		});

		builder.Services.Should().Contain(d => d.ServiceType == typeof(IRemoteConnection));
		builder.Services.Should().Contain(d => d.ServiceType == typeof(IRemoteConnectionFactory<TranscriptionConnection>));

	}

}
