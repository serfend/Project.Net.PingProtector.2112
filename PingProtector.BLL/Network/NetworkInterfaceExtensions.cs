using Common.Network;
using DotNet4.Utilities.UtilReg;
using IpSwitch.Helper;
using Microsoft.Win32;
using NetworkApi.NetworkManagement;
using NETworkManager.Models.Network;
using NLog;
using PingProtector.BLL.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
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
            g.ConfigureIpv4kByManagement(config);
            OnDhcpOpend?.Invoke(null, new DhcpOpendEventArgs(g));

            return true;
        }
        private static string reg_components = "DisabledComponents";
        private static int reg_disableBothIpv6Config = 0x11;
        public const string reg_ipv6Config = @"SYSTEM\CurrentControlSet\Services\Tcpip6\Parameters";
        /// <summary>
        /// 检查是否开启了ipv6
        /// </summary>
        /// <param name="g"></param>
        public static void CheckIpv6(this NetworkInterfaceInfo g, Action<object?, MessageEventArgs>? OnCmdMessage = null)
        {
            if (!g.IPv6ProtocolAvailable || g.Status != System.Net.NetworkInformation.OperationalStatus.Up) return;

            var reg = new Reg(reg_ipv6Config, RegDomain.LocalMachine);
            var current = long.Parse(reg.GetInfo(reg_components, "0"));
            if (current != reg_disableBothIpv6Config)
            {
                reg.SetInfo(reg_components, reg_disableBothIpv6Config, RegValueKind.DWord);
                OnCmdMessage?.Invoke(null, new MessageEventArgs(MessageEventArgs.MessageType.Info, "ipv6已从激活变为禁用"));
                DSAPI.文件.立即应用注册表更新();
                return;
            }
            // TODO 需要重启电脑
            OnCmdMessage?.Invoke(null, new MessageEventArgs(MessageEventArgs.MessageType.Error, "已设置但未生效,需要重启电脑"));
            // 在win7上不支持powershell此语法
            //var cmd = $"Disable-NetAdapterBinding -Name '{g.Name}' -ComponentID ms_tcpip6 -PassThru";
            //var runner = new CmdExecutor(CmdExecutor.Process_Powershell);
            //runner.OnMessage += (s, e) => OnCmdMessage?.Invoke(s, e);
            //runner.CmdRun("disable ipv6", cmd, null, true);
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
            var current = g.IPv4Gateway.FirstOrDefault()?.ToString()?.Ip2Int() ?? 0;
            var allowV4 = allowGates.Any(i =>
            {
                var range = i.Ipv4Range();
                return current >= range.Item1 && current <= range.Item2 || current == 0;
            });
            if (allowV4) return true;
            var config = g.ToConfig();
            config.Gateway = "1.1.1.1";
            g.ConfigureIpv4kByManagement(config);
            OnNetworkGatewayOutOfRange?.Invoke(null, new NetworkGatewayOutofRangeEventArgs(g));
            return false;
        }
        public static (uint, uint) Ipv4Range(this string ipRange)
        {
            var ips = ipRange.Split('-');
            return (ips[0].Ip2Int(), ips[1].Ip2Int());
        }
    }
}
