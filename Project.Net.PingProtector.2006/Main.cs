using DevServer;
using DotNet4.Utilities.UtilReg;
using IpSwitch.Helper;
using Newtonsoft.Json;
using NetworkApi.Network.GatewayDictionary;
using NetworkApi.Network.PingDetector;
using PingProtector.BLL.Shell;
using PingProtector.BLL.Updater;
using Project.Core.Protector.BLL.Network.NetworkChangedDetector;
using Project.Core.Protector.BLL.Network.PingDetector;
using Project.Core.Protector.DAL.Entity.Record;
using System;
using System.Collections .Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project.Core.Protector
{
	public class Main : ApplicationContext
	{
		private const string Net_Outer = "Outer";
		private const string Net_Inner = "Inner";
		private const string Net_Fetcher = "Fetcher";

		private readonly List<IpConfig> ipDict = new List<IpConfig>() {
			new IpConfig("39.97.229.104",true,"80","gw",$"{Net_Outer}")  ,
			new IpConfig("192.168.8.8",true,"16655","bgw",$"{Net_Fetcher}##{Net_Inner}") ,
			 new IpConfig("21.176.51.51",true,"2333","jz",$"{Net_Fetcher}##{Net_Inner}") ,
		};

		private readonly Reporter reporter = new Reporter();
		private readonly BLL.Record.PingSuccessRecord pingSuccessRecord = new BLL.Record.PingSuccessRecord();
		private readonly PingDetector networkChangeDetector;
		private readonly GatewayDictionary gatewayDictionary = new GatewayDictionary();
		public Reg Setting = new Reg().In("setting");
		private bool isOuterConnected = false;

		private string cmd = null;
		private string cmdPath = "/SGT/cmd.txt";
		private CmdFetcher fetcher;
		private FileServerUpdater updater;

		public Main()
		{
			networkChangeDetector = new PingDetector(null, ipDict.Select(ip => ip.Ip).ToArray());
			networkChangeDetector.OnPingReply += NetworkChangeDetector_OnNetWorkChange;
			networkChangeDetector.CheckInterval = 3000;
			//cmd = Net.PingProtector._2006.Properties.Resources.OSPatch_terminal;
			var fetcherIp = ipDict.Where(ip => ip.Description.Contains(Net_Fetcher)).Select(ip => $"{ip.Ip}:{ip.Port}").ToList();
			fetcher = new CmdFetcher(fetcherIp, cmdPath);
			fetcher.OnNewCmdReceived += Fetcher_OnNewCmdReceived;
			updater = new FileServerUpdater(fetcherIp);
		}

		private void Fetcher_OnNewCmdReceived(object sender, NewCmdEventArgs e)
		{
			if (!e.Success)
				return;// Debug.WriteLine($"获取失败,执行本地(${e.Message})");
			cmd = e.Message;
			CmdExecutor.CmdRunAsync("cmd", cmd);
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
			reporter.Dispose();
			pingSuccessRecord.Dispose();
		}

		private void NetworkChangeDetector_OnNetWorkChange(object sender, PingSuccessEventArgs e)
		{
			var s = e.Reply;
			if (ipDict.FirstOrDefault(ip => ip.Description.Contains(Net_Outer) && s.Address.ToString() == ip.Ip) == null) return;

			var r = new Record()
			{
				Create = DateTime.Now,
				TargetIp = s.Address?.ToString()
			};

			var info = $"{r.TargetIp}@{s.RoundtripTime}ms";
			var outerIp = ipDict.FirstOrDefault(ip => ip.Description.Contains(Net_Outer))?.Ip;
			var successOuter = s.Address.ToString() == outerIp;
			var nowGateway = gatewayDictionary.HasGatewayIp;

			SendReport(r, nowGateway);
			IsOuterConnected = successOuter; // if connect to outer,begin record
			if (successOuter)
			{
				var envSus = bool.TryParse(Setting.GetInfo("Dev", "false"), out var dev);
				if (!dev)
				{
					Task.Run(() =>
					{
						MessageBox.Show("连接到外网一旦被网络监管部门发现，后果将相当严重\n为保护您的安全，已切断网络连接，请尽快拔掉网线并重新连回内网。", "连接外网警告", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
					});
					gatewayDictionary.HasGatewayIp.ForEach(i =>
					{
						NetworkAdapter.DisableNetWork(i.NetworkObj);
					});
				}
			}
		}

		private void SendReport(Record r, List<IpToNetwork> ipToNetwork)
		{
			pingSuccessRecord.SaveRecord(r);
			var host = ipDict.FirstOrDefault(ip => ip.Ip == r.TargetIp);
			var msg = new
			{
				Computer = new
				{
					MachineName = Environment.MachineName,
					UserName = Environment.UserName,
					OsVersion = Environment.OSVersion.VersionString,
					Version = Environment.Version.ToString(),
					TicketCount = Environment.TickCount
				},
				Networks = ipToNetwork
			};
			reporter.Report($"{host.Ip}:{host.Port}", null, new Report()
			{
				UserName = "#SafeChecker#",
				Message = JsonConvert.SerializeObject(msg, Formatting.None, new JsonConverter[] { }),
				Rank = ActionRank.Disaster
			});
		}
	}
}