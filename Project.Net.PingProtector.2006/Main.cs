using Common.Extensions;
using DevServer;
using DotNet4.Utilities.UtilReg;
using Microsoft.AspNetCore.SignalR.Client;
using NetworkApi.Network.PingDetector;
using NetworkApi.NetworkInterfaceManagement;
using NetworkApi.NetworkManagement;
using NLog;
using PingProtector.BLL.Shell;
using Project.Core.Protector.BLL.Network.PingDetector;
using Project.Core.Protector.DAL.Entity.Record;
using Project.Net.PingProtector._2006;
using Project.Net.PingProtector._2006.Services;
using SignalRCommunicator;
using SignalRCommunicator.Proto;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.NetworkInformation;
using WinAPI;

namespace Project.Core.Protector
{
	public partial class Main : ApplicationContext
	{
		private const string Net_Outer = "Outer";
		private const string Net_Inner = "Inner";
		private const string Net_Fetcher = "Fetcher";

		private readonly List<IpConfig> ipDict = new()
		{
#if DEBUG
			new IpConfig("192.168.8.196", true, "2334", "csw", $"{Net_Fetcher}##{Net_Inner}"),
#endif
			new IpConfig("serfend.top", true, "443", "gw", $"{Net_Outer}"),
			new IpConfig("192.168.8.8", true, "443", "bgw", $"{Net_Fetcher}##{Net_Inner}"),
			new IpConfig("21.176.51.59", true, "443", "jz", $"{Net_Fetcher}##{Net_Inner}"),
		};

		private readonly NetworkInfo networkInfo = new();

		private readonly BLL.Record.PingSuccessRecord pingSuccessRecord = new();
		private readonly PingDetector networkChangeDetector;
		public Reg Setting = new Reg().In("setting");
		private bool isOuterConnected = false;
		private bool Warninging_Dhcp = false;
		private bool Warninging_Gateway = false;

		private string? cmd = null;

		//private readonly CmdFetcher fetcher;
		private Updater.Client.Updater appUpdater = new();

		public static Logger detectorLogger = LogManager.GetCurrentClassLogger().WithProperty("filename", LogServices.LogFile_Detector);

		private string pipeName = $"Inst_{Process.GetCurrentProcess().ProcessName}";
		private string selfInstaceId = Guid.NewGuid().ToString();
		private ProcessInstance processInstance;

		public Main()
		{
			LogServices.Init();
			detectorLogger.Log<string>(LogLevel.Info, "start");
			RegisterConfigration.Configuration.IsRunning = true;
			processInstance = new ProcessInstance(pipeName);
			processInstance.CheckInstaceByNamedPipe(null, () =>
			{
				detectorLogger.Warn($"新的实例已启动，关闭老实例@{selfInstaceId}");
				Environment.Exit(0);
			});
			networkChangeDetector = new PingDetector(null, ipDict.Select(ip => ip.Ip).ToArray());
			var fetcherIp = ipDict.Where(ip => ip.Description != null && ip.Description.Contains(Net_Fetcher)).Select(ip => $"{ip.Ip}:{ip.Port}").ToList();
			// fetcher = new CmdFetcher(fetcherIp, cmdPath);
			Init();
			Task.Run(() =>
			{
				var tip = ProjectI18n.Default?.Current?.Notification?.StartUpTip;
				IntPtr.Zero.ShowMessageBox($"{tip?.Content ?? "已启动"}{appUpdater.CurrentVersion}@{selfInstaceId}", tip?.Title ?? BrandName, WTSapi32.DialogStyle.MB_ICONINFORMATION);
			});
		}

		private void Init()
		{
			networkChangeDetector.OnPingReply += NetworkChangeDetector_OnPingReply;
			networkChangeDetector.OnTick += (s, e) =>
			{
				RegisterConfigration.Configuration.CurrentRunningInstanceActive = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				var isRunning = RegisterConfigration.Configuration.IsRunning;
				if (!isRunning)
				{
					detectorLogger.Warn("运行已被要求停止");
					Environment.Exit(0);
				}
				var interfaces = networkInfo.CheckInterfaces();
			};
			networkChangeDetector.CheckInterval = 3000;
			TipInit();
			NetworkChange.NetworkAddressChanged += (s, e) =>
			{
				var content = "检测到网络ip变化";
				detectorLogger.Warn(content);
				//IntPtr.Zero.ShowMessageBox(content, "监测");
				networkInfo.CheckInterfaces();
			};
			NetworkChange.NetworkAvailabilityChanged += (s, e) =>
			{
				var content = "检测到网络ip变化";
				detectorLogger.Warn(content);
				//IntPtr.Zero.ShowMessageBox(content, "监测");
				networkInfo.CheckInterfaces();
			};
			networkInfo.CheckInterfaces();
		}

		private void Fetcher_OnNewCmdReceived(object? sender, NewCmdEventArgs e)
		{
			if (!e.Success)
				return;// Debug.WriteLine($"获取失败,执行本地(${e.Message})");
			cmd = e.Message;
			if (cmd == null || cmd == string.Empty) return;
			new CmdExecutor().CmdRunAsync("cmd_required", cmd);
		}

		private bool IsOuterConnected
		{
			get => isOuterConnected; set
			{
				isOuterConnected = value;
				pingSuccessRecord.Enabled = value;
			}
		}

		protected void OnClosed(EventArgs e)
		{
			pingSuccessRecord.Dispose();
		}

		private void NetworkChangeDetector_OnPingReply(object? sender, PingSuccessEventArgs e)
		{
			var s = e.Reply;
			// 注意当断网状态下时返回success，但ip是本地的可能性
			if (ipDict.FirstOrDefault(ip => ip.Description != null && ip.Description.Contains(Net_Outer)) == null) return;

			var r = new Record()
			{
				Create = DateTime.Now,
				TargetIp = s.Address?.ToString(),
				TargetHost = e.Host
			};

			var info = $"{r.TargetIp}@{s.RoundtripTime}ms";
			var outerIp = ipDict.FirstOrDefault(ip => ip.Description != null && ip.Description.Contains(Net_Outer))?.Ip;
			var successOuter = s.Address?.ToString() == outerIp;

			SendReport(r);

			IsOuterConnected = successOuter; // if connect to outer,begin record
			if (successOuter)
			{
				var envSus = bool.TryParse(Setting.GetInfo("Dev", "false"), out var dev);
				if (!dev)
				{
					var interfaces = networkInfo.CheckInterfaces();
					StartOutterAction(interfaces);
				}
			}
		}
	}

	/// <summary>
	/// signalr communicator
	/// </summary>
	public partial class Main
	{
		private struct SignalRConnection
		{
			public SignalrCommunicator Connection;
			public Report<ClientDeviceInfoDTO>? LastData;
			public DateTime? LastUpdate;
		}

		private ConcurrentDictionary<string, SignalRConnection> signalrConncetions = new();

		/// <summary>
		/// 初始化signalr-connection
		/// </summary>
		/// <param name="connectionTarget"></param>
		/// <returns></returns>
		private static SignalRConnection InitConnection(string connectionTarget)
		{
			var r = new SignalRConnection()
			{
				Connection = new SignalrCommunicator(connectionTarget),
			};
			var serverUpdateHostUpdate = r.Connection.connection.On<string>("SetUpdateHost", t =>
			{
				detectorLogger.Warn($"SetUpdateHost:{t}");
				RegisterConfigration.Configuration.ServerHost = t;
			});
			var setUpdateCheckInterval = r.Connection.connection.On<string>("SetUpdateCheckInterval", t =>
			{
				detectorLogger.Warn($"SetUpdateCheckInterval:{t}");
				_ = int.TryParse(t, out var s);
				if (s > 0)
					RegisterConfigration.Configuration.UpdateCheckInterval = s;
			});
			return r;
		}

		private void SendReport(Record r)
		{
			pingSuccessRecord.SaveRecord(r);
			var host = ipDict.FirstOrDefault(ip => ip.Ip == r.TargetHost);
			if (host == null) return;

			var connectionTarget = $"{host.Ip}:{host.Port}";
			if (!signalrConncetions.ContainsKey(connectionTarget))
			{
				signalrConncetions[connectionTarget] = InitConnection(connectionTarget);
			}

			var ipToNetwork = networkInfo.CheckInterfaces();
			var (c, data) = CheckIfShouldSend(ipToNetwork, connectionTarget);
			if (data == null) return;
			var tryTime = 1;
			while (!c.Connection.ReportClientInfo(data).Result && tryTime-- > 0)
			{
				Debug.Print($"发送失败:{tryTime}");
				Thread.Sleep(1000);
			}
		}

		private (SignalRConnection, Report<ClientDeviceInfoDTO>?) CheckIfShouldSend(List<NetworkInterfaceInfo> ipToNetwork, string connectionTarget)
		{
			#region init data

			var msg = new ClientDeviceInfoDTO
			{
				Computer = new ClientComputerInfoDTO
				{
					MachineName = Environment.MachineName,
					UserName = Environment.UserName,
					OsVersion = Environment.OSVersion.VersionString,
					Version = Environment.Version.ToString(),
					TicketCount = Environment.TickCount
				},
				Network = new ClientNetworkInfoDTO() { Interfaces = ipToNetwork?.Select(i => i?.ToDto()) }
			};
			var data = new Report<ClientDeviceInfoDTO>()
			{
				UserName = new Reporter().Uid,
				Message = msg,
				Device = $"ClientDesktop {appUpdater.CurrentVersion}",
				Rank = ActionRank.Debug
			};

			#endregion init data

			var c = signalrConncetions[connectionTarget];
			if (c.LastUpdate < DateTime.Today && new Random().Next(1000) > 990) { } // 新的一天，择机同步消息
			else if (c.LastData?.Message?.Equals(data.Message) ?? false) return (c, null);
			c.LastUpdate = DateTime.Today;
			c.LastData = data;
			signalrConncetions[connectionTarget] = c;
			return (c, data);
		}
	}
}