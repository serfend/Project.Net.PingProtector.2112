using Configuration.FileHelper;
using NetworkApi.NetworkInterfaceManagement;
using Newtonsoft.Json;
using NLog;
using PingProtector.BLL.Network;
using static Common.CmdShellHelper.MessageEventArgs;

namespace Project.Net.PingProtector._2006.Services
{
	public class NetworkInfo
	{
		public static Logger detectorLogger = LogManager.GetLogger(LogServices.LogFile_Detector);
		private List<string>? listGateways;

		public NetworkInfo()
		{
			InitAllowedList();
		}

		private void InitAllowedList()
		{
			listGateways = null;
			try
			{
				var f = new CiperFile() { Path = "./conf/configuration_gate.dat" };
				f.Load();
				listGateways = f.Content == null ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(f.Content);
			}
			catch (Exception)
			{
			}
		}

		private DateTime lastInterfacesUpdateTime = DateTime.MinValue;
		private List<NetworkInterfaceInfo>? interfaces;

		public List<NetworkInterfaceInfo> Interfaces
		{
			get
			{
				var now = DateTime.Now;
				if (now.Subtract(lastInterfacesUpdateTime).TotalSeconds > 3)
				{
					lastInterfacesUpdateTime = now;
					interfaces = NetworkInterface.GetNetworkInterfaces();
				}
				return interfaces ?? new List<NetworkInterfaceInfo>();
			}
		}

		public List<NetworkInterfaceInfo> CheckInterfaces()
		{
			var network_interface = new NetworkInterface();
#if !DEBUG
			Interfaces.ForEach(g =>
			{
				g.CheckDhcpConfigure();
				g.CheckGatewayRange(listGateways);
				g.CheckIpv6((s, e) =>
				{
					detectorLogger.Log(e.Type == MessageType.Info ? LogLevel.Info : LogLevel.Error, e.Message);
				});
			});
#endif
			return Interfaces;
		}
	}
}