using Configuration.FileHelper;
using NETworkManager.Models.Network;
using Newtonsoft.Json;
using NLog;
using PingProtector.BLL.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PingProtector.BLL.Shell.MessageEventArgs;

namespace Project.Net.PingProtector._2006.Services
{
    public static class NetworkInterfaceInfoExtensions
    {
        public static string ToSummary(this NetworkInterfaceInfo? info)
        {
            if (info == null) return "[无效的网卡信息]";
            var ips = string.Join(',', info.IPv4Address.Select(i => i.ToString()));
            var gates = string.Join(',', info.IPv4Gateway.Select(i => i.ToString()));
            var name = info.Name;
            return $"{name}:{ips}(gateway:{gates})";
        }
        public static string ToDetail(this NetworkInterfaceInfo? i)
        {
            if (i == null) return i.ToSummary();
            var item = new
            {
                Name = i.Name,
                Ipv4 = i.IPv4Address.Select(p => $"{p.Item1}/{p.Item2}"),
                Ipv4Gateway = i.IPv4Gateway.Select(p => $"{p}"),
                Ipv6 = i.IPv6Address.Select(p => $"{p}"),
                Ipv6Gateway = i.IPv6Gateway.Select(p => $"{p}"),
                Description = i.Description,
                Type = i.Type,
                Speed = i.Speed,
                IsOperational = i.IsOperational,
                PhysicalAddress = i.PhysicalAddress.ToString(),
                Status = i.Status,
                IPv4ProtocolAvailable = i.IPv4ProtocolAvailable,
                IPv6ProtocolAvailable = i.IPv6ProtocolAvailable,
                DhcpEnabled = i.DhcpEnabled,
                Dns = i.DNSServer.Select(p => p.ToString()),
                Id = i.Id
            };
            return System.Text.Json.JsonSerializer.Serialize(item);
        }
    }
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
                var f = new CiperFile() { Path = "configuration_gate.dat" };
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

            interfaces.ForEach(g =>
            {
                g.CheckDhcpConfigure();
                g.CheckGatewayRange(listGateways);
                g.CheckIpv6((s, e) =>
                {
                    detectorLogger.Log(e.Type == MessageType.Info ? LogLevel.Info : LogLevel.Error, e.Message);
                });
            });
            return interfaces;
        }
    }
}
