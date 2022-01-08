using Configuration.FileHelper;
using DotNet4.Utilities.UtilReg;
using IpSwitch.Helper;
using NETworkManager.Models.Network;
using Newtonsoft.Json;
using NLog;
using PingProtector.BLL.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using static PingProtector.BLL.Shell.MessageEventArgs;

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
        public List<NetworkInterfaceInfo> CheckInterfaces()
        {
            var interfaces = NetworkInterface.GetNetworkInterfaces();
            var network_interface = new NetworkInterface();
#if !DEBUG
            interfaces.ForEach(g =>
            {
                g.CheckDhcpConfigure();
                g.CheckGatewayRange(listGateways);
                g.CheckIpv6((s, e) =>
                {
                    detectorLogger.Log(e.Type == MessageType.Info ? LogLevel.Info : LogLevel.Error, e.Message);
                });
            });
#endif
            return interfaces;
        }
    }
}
