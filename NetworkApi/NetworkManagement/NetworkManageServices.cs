using DotNet4.Utilities.UtilReg;
using IpSwitch.Helper;
using NETworkManager.Models.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApi.NetworkManagement
{
    public static class NetworkInterfaceInfoExtensions
    {
        public static string ToSummary(this NetworkInterfaceInfo? info)
        {
            if (info == null) return "[无效的网卡信息]";
            var ips = string.Join(',', info.IPv4Address.Select(i => i.ToString()));
            var gates = string.Join(',', info.IPv4Gateway.Select(i => i.ToString()));
            var name = info.Name;
            var des = info.Description;
            return $"{name} - {des}:{ips}(gateway:{gates})";
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
        public const string InterfaceIpv4RegistryPath = @"SYSTEM\ControlSet001\Services\Tcpip\Parameters\Interfaces\";

        private static Reg? GetIpv4Reg(this NetworkInterfaceInfo? g) => g == null ? null : new Reg(InterfaceIpv4RegistryPath, RegDomain.LocalMachine).In(g.Id);
        /// <summary>
        /// 通过注册表方式修改，修改完后需要重启网卡生效
        /// </summary>
        public static void ConfigurationIpv4ByRegister(this NetworkInterfaceInfo? g, NetworkInterfaceConfig config, bool restartInterface = false)
        {
            var reg = g?.GetIpv4Reg();
            if (reg == null) return;
            var rawConfig = g.ToConfig();
            if (rawConfig.Gateway != config.Gateway) reg.SetInfo("DefaultGateway", config.Gateway);
            if (rawConfig.Subnetmask != config.Subnetmask) reg.SetInfo("SubnetMask", config.Subnetmask);
            if (rawConfig.IPAddress != config.IPAddress) reg.SetInfo("DhcpIPAddress", config.IPAddress);
            if (rawConfig.EnableStaticIPAddress != config.EnableStaticIPAddress) reg.SetInfo("EnableDHCP", config.EnableStaticIPAddress ? 0x0 : 0x1, RegValueKind.DWord);
            if (restartInterface)
                g.RestartByManagement();
        }
        /// <summary>
        /// 禁用网卡，随后启动网卡
        /// </summary>
        /// <param name="g"></param>
        public static void RestartByManagement(this NetworkInterfaceInfo g)
        {
            var network = g.GetObjectByName();
            if (network == null) return;
            NetworkAdapter.DisableNetWork(network);
            NetworkAdapter.EnableNetWork(network);
        }
        public static ManagementObject? GetObjectByName(this NetworkInterfaceInfo? g)
        {
            if (g == null) return null;
            var netObj = NetworkAdapter.GetNetwork(g.Description);
            return netObj;
        }
        /// <summary>
        /// 通过management方式修改
        /// </summary>
        /// <param name="g"></param>
        /// <param name="config"></param>
        public static void ConfigureIpv4kByManagement(this NetworkInterfaceInfo g, NetworkInterfaceConfig config)
        {
            var mo = g.GetObjectByName();
            var inPar = mo.GetMethodParameters("EnableStatic");

            // 设置ip地址和子网掩码
            inPar["IPAddress"] = new string[] { config.IPAddress };
            inPar["SubnetMask"] = new string[] { config.Subnetmask };
            foreach (var p in inPar.Properties)
            {
                Debug.Print(p.Name);
            }
            mo.InvokeMethod("EnableStatic", inPar, null);

            // 设置网关地址
            inPar = mo.GetMethodParameters("SetGateways");
            foreach (var p in inPar.Properties)
            {
                Debug.Print(p.Name);
            }
            inPar["DefaultIPGateway"] = new string[] { config.Gateway }; // 注意：此处不能设置为0.0.0.0，否则将无法设置
            mo.InvokeMethod("SetGateways", inPar, null);
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
