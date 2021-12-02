using Common.Network;
using NETworkManager.Models.Network;
using NLog;
using PingProtector.BLL.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.BLL.Network
{
    public class NetworkEventArgs : EventArgs
    {
        public NetworkInterfaceInfo Interface { get; set; }

        public NetworkEventArgs(NetworkInterfaceInfo inter)
        {
            Interface = inter;
        }
    }
    public class NetworkGatewayOutofRangeEventArgs : NetworkEventArgs
    {
        public NetworkGatewayOutofRangeEventArgs(NetworkInterfaceInfo inter) : base(inter)
        {
        }
    }
    public class DhcpOpendEventArgs : NetworkEventArgs
    {
        public DhcpOpendEventArgs(NetworkInterfaceInfo inter) : base(inter)
        {
        }
    }
    public static class NetworkInterfaceExtensions
    {
        public static event EventHandler<NetworkGatewayOutofRangeEventArgs>? OnNetworkGatewayOutOfRange;
        public static event EventHandler<DhcpOpendEventArgs>? OnDhcpOpend;
        public static NetworkInterface Interface = new NetworkInterface();
        /// <summary>
        /// 检查是否是活跃的dhcp网卡
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool CheckDhcpConfigure(this NetworkInterfaceInfo g)
        {
            if (!g.DhcpEnabled || g.Status != System.Net.NetworkInformation.OperationalStatus.Up) return false;
            var config = g.ToConfig();
            OnDhcpOpend?.Invoke(null, new DhcpOpendEventArgs(g));
            Interface.ConfigureNetworkInterface(config); // 使用powershell方式执行

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            var parIPSetting = mc.GetMethodParameters("EnableStatic");//对于有参数的Win32_NetworkAdapterConfiguration类的方法，得先用GetMethodParameters方法来获得参数对象，然后再给参数赋值。
            parIPSetting["IPAddress"] = new string[] { config.IPAddress };
            parIPSetting["SubnetMask"] = new string[] { config.Subnetmask };
            mc.InvokeMethod("EnableStatic", parIPSetting, null);//这是一个设置IP地址及子网掩码的例子
            return true;
        }
        /// <summary>
        /// 检查是否开启了ipv6
        /// </summary>
        /// <param name="g"></param>
        public static void CheckIpv6(this NetworkInterfaceInfo g, Action<object?, MessageEventArgs>? OnCmdMessage = null)
        {
            if (!g.IPv6ProtocolAvailable || g.Status != System.Net.NetworkInformation.OperationalStatus.Up ) return;
            
            var cmd = $"Disable-NetAdapterBinding -Name '{g.Name}' -ComponentID ms_tcpip6 -PassThru";
            var runner = new CmdExecutor(CmdExecutor.Process_Powershell);
            runner.OnMessage += (s, e) => OnCmdMessage?.Invoke(s, e);
            runner.CmdRun("disable ipv6", cmd, null, true);
        }
        /// <summary>
        /// 检查是否是合规的网关地址范围
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static bool CheckGatewayRange(this NetworkInterfaceInfo g, List<string>? allowGates)
        {
            allowGates ??= new List<string>();
            if (g.Status != System.Net.NetworkInformation.OperationalStatus.Up) return true;
            if (g.Name.ToLower().Contains("vmware")) return true; // 不检查vmware
            var allowV4 = allowGates.Any(i =>
            {
                var range = i.Ipv4Range();
                var current = g.IPv4Gateway.FirstOrDefault()?.ToString()?.Ip2Int() ?? 0;
                return current >= range.Item1 && current <= range.Item2 || current == 0;
            });
            var allowV6 = !g.IPv6ProtocolAvailable;
            if (allowV4 && !allowV6) return true;
            var config = g.ToConfig();
            config.Gateway = "0.0.0.0";
            OnNetworkGatewayOutOfRange?.Invoke(null, new NetworkGatewayOutofRangeEventArgs(g));
            Interface.ConfigureNetworkInterface(config);
            return false;
        }
        public static (uint, uint) Ipv4Range(this string ipRange)
        {
            var ips = ipRange.Split('-');
            return (ips[0].Ip2Int(), ips[1].Ip2Int());
        }
        public static NetworkInterfaceConfig ToConfig(this NetworkInterfaceInfo g)
        {
            var ipv4 = g.IPv4Address.FirstOrDefault();
            var config = new NetworkInterfaceConfig()
            {
                Name = g.Name,
                EnableStaticIPAddress = true, // 使用静态ip
                IPAddress = ipv4?.Item1?.ToString(),
                Subnetmask = ipv4?.Item2?.ToString(),
                Gateway = g.IPv4Gateway.FirstOrDefault()?.ToString(),
                EnableStaticDNS = false,
                PrimaryDNSServer = null,
                SecondaryDNSServer = null,
                IpVersion = g.IPv4ProtocolAvailable ? IpVersionConfig.ipv4 : IpVersionConfig.ipv6
            };
            return config;
        }
    }
}
