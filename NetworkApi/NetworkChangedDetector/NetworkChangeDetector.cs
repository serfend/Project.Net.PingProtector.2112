using NETworkManager.Models.Network;
using NetworkApi.Network.NetworkChangedDetector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetworkInterface = NETworkManager.Models.Network.NetworkInterface;
using Ping = System.Net.NetworkInformation.Ping;

namespace Project.Core.Protector.BLL.Network.NetworkChangedDetector
{
	public class NetworkChangeDetector
	{
		private readonly List<string> detectIp;

		public NetStatus Host { get; private set; } = new NetStatus() { Type = NetType.Host };

		public NetStatus GateWay { get; private set; } = new NetStatus() { Type = NetType.Gateway };
		public NetStatus Internet { get; private set; } = new NetStatus() { Type = NetType.Internet };

		public event EventHandler<NetworkChangeArgs> OnNetWorkChange;

		public NetworkChangeDetector(List<string> detectIp)
		{
			// Detect if network address or status changed...
			NetworkChange.NetworkAvailabilityChanged += (sender, args) => CheckConnectionAsync();
			NetworkChange.NetworkAddressChanged += (sender, args) => CheckConnectionAsync();
			Task.Run(() =>
			{
				Task.Delay(2000);
				CheckConnection();
			});
			Task.Run(() =>
			{
				RunLongTerm();
			});
			this.detectIp = detectIp;
		}

		private void RunLongTerm()
		{
			int count = 0;
			while (count++ > 0)
			{
				if (count == 300)
				{
					count = 1;
					CheckConnectionAsync();
				}
				Thread.Sleep(1000);
			}
		}

		#region method

		public void CheckConnectionAsync()
		{
			Task.Run(() => CheckConnection());
		}

		public void CheckConnection()
		{
			if (Host.IsCheckRunning)
				return;

			// Reset
			Host.IsCheckRunning = true;
			Host.IsCheckComplete = false;
			Host.Status = "";
			Host.Log = NetStatus.NOT_Reachable;
			Host.ConnectionState = ConnectionState.None;
			Host.IPAddress = null;
			Host.Hostname = "";

			GateWay.IsCheckRunning = true;
			GateWay.IsCheckComplete = false;
			GateWay.Status = "";
			GateWay.Log = NetStatus.NOT_Reachable;
			GateWay.ConnectionState = ConnectionState.None;
			GateWay.IPAddress = null;
			GateWay.Hostname = "";

			Internet.IsCheckRunning = true;
			Internet.IsCheckComplete = false;
			Internet.Status = "";
			Internet.Log = NetStatus.NOT_Reachable;
			Internet.ConnectionState = ConnectionState.None;
			Internet.IPAddress = null;
			Internet.Hostname = "";

			#region Host

			CheckHost();

			#endregion Host

			#region Gateway / Router

			CheckGateway();

			#endregion Gateway / Router

			#region Internet

			// 6) Check if internet is reachable via icmp to a public ip address
			foreach (var ip in detectIp)
			{
				var r = CheckInternet(ip);
				if (r) break;
			}

			#endregion Internet
		}

		private void CheckHost()
		{
			// 1) Check tcp/ip stack --> Ping to 127.0.0.1
			var hostIPAddress = "127.0.0.1";

			using (var ping = new Ping())
			{
				for (var i = 0; i < 2; i++)
				{
					try
					{
						var pingReply = ping.Send(IPAddress.Parse(hostIPAddress));

						if (pingReply == null || pingReply.Status != IPStatus.Success)
							continue;

						Host.Log = pingReply.RoundtripTime;

						break;
					}
					catch (PingException)
					{
					}
				}
			}

			if (Host.Log == NetStatus.NOT_Reachable)
			{
				Host.ConnectionState = ConnectionState.Critical;
				AddToDetails(Host, ConnectionState.Critical, $"连接到{hostIPAddress}失败");

				Host.IsCheckRunning = false;
				Internet.IsCheckRunning = false;
				GateWay.IsCheckRunning = false;
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Host));
				return;
			}

			Host.ConnectionState = ConnectionState.OK;
			AddToDetails(Host, ConnectionState.OK, $"连接到{hostIPAddress}成功");
			OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Host));

			// 2) Detect local ip address
			try
			{
				var ip = IPAddress.Parse("1.1.1.1");
				Host.IPAddress = NetworkInterface.DetectLocalIPAddressBasedOnRouting(ip)?.ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"localhost connect:{ex.Message}");
			}

			if (Host.IPAddress == null)
			{
				Host.ConnectionState = ConnectionState.Critical;
				AddToDetails(Host, ConnectionState.Critical, "获取本地ip失败");

				Host.IsCheckRunning = false;
				GateWay.IsCheckRunning = false;
				Internet.IsCheckRunning = false;
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Host));

				return;
			}

			AddToDetails(Host, ConnectionState.OK, $"获取到本地ip{Host.IPAddress}");

			// 3) Check dns for local host
			try
			{
				Host.Hostname = Dns.GetHostEntry(Host.IPAddress).HostName;

				AddToDetails(Host, ConnectionState.OK, $"解析到本地ip:{Host.IPAddress}");
			}
			catch (SocketException)
			{
				Host.ConnectionState = ConnectionState.Warning;
				AddToDetails(Host, ConnectionState.Warning, $"DNS解析异常");
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Host));
			}

			Host.IsCheckRunning = false;
			Host.IsCheckComplete = true;
			OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Host));
		}

		private void CheckGateway()
		{
			// 4) Detect gateway ip address
			try
			{
				GateWay.IPAddress = NetworkInterface.DetectGatewayBasedOnLocalIPAddress(IPAddress.Parse(Host.IPAddress)).ToString();
			}
			catch (Exception)
			{
				// ignored
			}

			if (GateWay.IPAddress == null)
			{
				AddToDetails(GateWay, ConnectionState.Critical, "获取网关失败");

				GateWay.IsCheckRunning = false;
				Internet.IsCheckRunning = false;
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(GateWay));

				return;
			}

			AddToDetails(GateWay, ConnectionState.OK, $"获取到网关ip:{GateWay.IPAddress}");

			// 4) Check if gateway is reachable via ICMP
			using (var ping = new Ping())
			{
				for (var i = 0; i < 1; i++)
				{
					try
					{
						var pingReply = ping.Send(GateWay.IPAddress);

						if (pingReply == null || pingReply.Status != IPStatus.Success)
							continue;
						GateWay.Log = pingReply.RoundtripTime;

						break;
					}
					catch (PingException)
					{
						// ignore
					}
				}
			}

			if (GateWay.Log == NetStatus.NOT_Reachable)
			{
				AddToDetails(GateWay, ConnectionState.Critical, $"无法连接到网关via ICMP :{GateWay.IPAddress}");

				GateWay.IsCheckRunning = false;
				Internet.IsCheckRunning = false;
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(GateWay));

				return;
			}

			GateWay.ConnectionState = ConnectionState.OK;
			AddToDetails(GateWay, ConnectionState.OK, $"已连接到网关{GateWay.IPAddress}");
			OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(GateWay));

			// 5) Check dns for gateway
			try
			{
				GateWay.Hostname = Dns.GetHostEntry(GateWay.IPAddress).HostName;

				AddToDetails(GateWay, ConnectionState.OK, "解析网关成功");
			}
			catch (SocketException)
			{
				GateWay.ConnectionState = ConnectionState.Warning;
				AddToDetails(GateWay, ConnectionState.Warning, "解析网关发生异常");
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(GateWay));
			}

			GateWay.IsCheckRunning = false;
			GateWay.IsCheckComplete = true;
			OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(GateWay));
		}

		private bool CheckInternet(string ip)
		{
			using (var ping = new Ping())
			{
				try
				{
					var pingReply = ping.Send(IPAddress.Parse(ip));

					if (pingReply == null || pingReply.Status != IPStatus.Success)
						Internet.Log = NetStatus.NOT_Reachable;
					else
					{
						Internet.Log = pingReply.RoundtripTime;
						Internet.IPAddress = ip;
					}
				}
				catch (PingException)
				{
				}
			}

			if (Internet.Log == NetStatus.NOT_Reachable)
			{
				AddToDetails(Internet, ConnectionState.Critical, "连接到网络异常");
				Internet.IsCheckRunning = false;
				OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Internet));

				return false;
			}

			Internet.ConnectionState = ConnectionState.OK;
			AddToDetails(Internet, ConnectionState.OK, "已连接到网络");
			OnNetWorkChange?.Invoke(this, new NetworkChangeArgs(Internet));

			Internet.IsCheckRunning = false;
			Internet.IsCheckComplete = true;
			return true;
		}

		private void AddToDetails(NetStatus status, ConnectionState state, string message)
		{
			if (!string.IsNullOrEmpty(status.Status))
				status.Status += Environment.NewLine;
			var msg = $"[{state}] {message}";
			Debug.WriteLine($"network connect detail:{msg}");

			status.Status += msg;
		}

		#endregion method
	}
}