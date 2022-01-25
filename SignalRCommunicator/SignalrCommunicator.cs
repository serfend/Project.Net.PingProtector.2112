using DevServer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRCommunicator.Events;
using SignalRCommunicator.Proto;

namespace SignalRCommunicator
{
	public class DefaultRetryPolicy : IRetryPolicy
	{
		public TimeSpan? NextRetryDelay(RetryContext retryContext)
		{
			var time = Math.Pow(retryContext.PreviousRetryCount, 1.5);
			return TimeSpan.FromSeconds(time);
		}
	}

	public class SignalrCommunicator
	{
		public HubConnection connection;
		public static string TargetHub = "/ws/deviceClient";

		public SignalrCommunicator(string ip)
		{
			var url = $"wss://{ip}{TargetHub}";
			connection = new HubConnectionBuilder()
				.WithUrl(url, opts =>
				{
					opts.HttpMessageHandlerFactory = (message) =>
					{
						if (message is HttpClientHandler handler)
							handler.ServerCertificateCustomValidationCallback +=
								(sender, certificate, chain, sslPolicyErrors) => { return true; };
						return message;
					};
					opts.Transports = HttpTransportType.ServerSentEvents | HttpTransportType.WebSockets | HttpTransportType.LongPolling | HttpTransportType.None;
				})
				.WithAutomaticReconnect(new DefaultRetryPolicy())
				.Build();
			connection.Closed += (error) =>
		   {
			   OnConnectionLost?.Invoke(this, new ConnectionLostEventArgs(error));
			   return Task.CompletedTask;
		   };
			connection.Reconnected += e =>
		   {
			   OnConnectionRebuild?.Invoke(this, new ConnectionRebuildEventArgs(e));
			   return Task.CompletedTask;
		   };
			var heartbeat = connection.On<string>("Hi", (t) => { Console.WriteLine("Message"); });
			var messageCommon = connection.On<string>("ReceiveMessage", t => { Console.WriteLine("Message"); });
			connection.StartAsync();
		}

		public event EventHandler<MessageFailEventArgs>? OnMessageFail;

		public event EventHandler<ConnectionLostEventArgs>? OnConnectionLost;

		public event EventHandler<ConnectionRebuildEventArgs>? OnConnectionRebuild;

		public Task<bool> ReportClientInfo<T>(Report<T> content) where T : class
		{
			if (content == null) return Task.FromResult(false);
			if (content.UserName == null) content.UserName = String.Empty;
			if (!content.UserName.StartsWith(GlobalFlag.UserNamePrefix)) content.UserName = $"{GlobalFlag.UserNamePrefix}{content.UserName}";
			if (connection.State == HubConnectionState.Connected)
			{
				try
				{
					var result = connection.InvokeAsync<object>("Hi", content).Result;
					return Task.FromResult(null != result);
				}
				catch (Exception ex)
				{
					OnMessageFail?.Invoke(this, new MessageFailEventArgs(ex));
					return Task.FromResult(false);
				}
			}
			return Task.FromResult(false);
		}
	}
}