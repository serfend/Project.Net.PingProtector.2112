using DevServer;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRCommunicator
{
	public class SignalrCommunicator
	{
		public HubConnection connection;
		public static string TargetHub = "/ws/DeviceClient";
		public SignalrCommunicator(string ip)
		{
			var url = $"{ip}{TargetHub}";
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
				})
				.WithAutomaticReconnect()
				.Build();
			connection.Closed += async (error) =>
			{
				await Task.Delay(new Random().Next(0, 5) * 1000);
				await connection.StartAsync();
			};
			connection.Reconnected += async e =>
			{
				await connection.SendAsync("Hi");
			};
			var heartbeat = connection.On<string>("Hi", (t) => { Console.WriteLine("Message"); });
			var messageCommon = connection.On<string>("ReceiveMessage", t => { Console.WriteLine("Message"); });
			connection.StartAsync();
		}
		public Task<bool> ReportClientInfo<T>(Report<T> content) where T : class
		{
			if (connection.State == HubConnectionState.Connected)
			{
				var result = connection.InvokeAsync<dynamic>("Hi", content);
				return Task.FromResult(null != result.Result);
			}
			return Task.FromResult(false);
		}

	}
}