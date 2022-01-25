﻿using Common.PowershellHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApi.NetworkInterfaceManagement
{
	public class NetworkInterface
	{
		#region Events

		public event EventHandler? UserHasCanceled;

		protected virtual void OnUserHasCanceled()
		{
			UserHasCanceled?.Invoke(this, EventArgs.Empty);
		}

		#endregion Events

		#region Methods

		public static Task<List<NetworkInterfaceInfo>> GetNetworkInterfacesAsync()
		{
			return Task.Run(() => GetNetworkInterfaces());
		}

		public static List<NetworkInterfaceInfo> GetNetworkInterfaces()
		{
			var listNetworkInterfaceInfo = new List<NetworkInterfaceInfo>();

			foreach (var networkInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
			{
				// NetworkInterfaceType 53 is proprietary virtual/internal interface
				// https://docs.microsoft.com/en-us/windows-hardware/drivers/network/ndis-interface-types
				if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && (int)networkInterface.NetworkInterfaceType != 53)
					continue;

				var listIPv4Address = new List<Tuple<IPAddress, IPAddress>>();
				var listIPv6AddressLinkLocal = new List<IPAddress>();
				var listIPv6Address = new List<IPAddress>();

				var dhcpLeaseObtained = new DateTime();
				var dhcpLeaseExpires = new DateTime();

				var ipProperties = networkInterface.GetIPProperties();

				foreach (var unicastIPAddrInfo in ipProperties.UnicastAddresses)
				{
					switch (unicastIPAddrInfo.Address.AddressFamily)
					{
						case AddressFamily.InterNetwork:

							listIPv4Address.Add(new Tuple<IPAddress, IPAddress>(unicastIPAddrInfo.Address, unicastIPAddrInfo.IPv4Mask));
							dhcpLeaseExpires = (DateTime.UtcNow + TimeSpan.FromSeconds(unicastIPAddrInfo.AddressPreferredLifetime)).ToLocalTime();
							dhcpLeaseObtained = (DateTime.UtcNow + TimeSpan.FromSeconds(unicastIPAddrInfo.AddressValidLifetime) - TimeSpan.FromSeconds(unicastIPAddrInfo.DhcpLeaseLifetime)).ToLocalTime();
							break;

						case AddressFamily.InterNetworkV6 when unicastIPAddrInfo.Address.IsIPv6LinkLocal:
							listIPv6AddressLinkLocal.Add(unicastIPAddrInfo.Address);
							break;

						case AddressFamily.InterNetworkV6:
							listIPv6Address.Add(unicastIPAddrInfo.Address);
							break;
					}
				}

				var listIPv4Gateway = new List<IPAddress>();
				var listIPv6Gateway = new List<IPAddress>();

				foreach (var gatewayIPAddrInfo in ipProperties.GatewayAddresses)
				{
					switch (gatewayIPAddrInfo.Address.AddressFamily)
					{
						case AddressFamily.InterNetwork:
							listIPv4Gateway.Add(gatewayIPAddrInfo.Address);
							break;

						case AddressFamily.InterNetworkV6:
							listIPv6Gateway.Add(gatewayIPAddrInfo.Address);
							break;
					}
				}

				var listDhcpServer = new List<IPAddress>();

				foreach (var dhcpServerIPAddress in ipProperties.DhcpServerAddresses)
				{
					if (dhcpServerIPAddress.AddressFamily == AddressFamily.InterNetwork)
						listDhcpServer.Add(dhcpServerIPAddress);
				}

				// Check if autoconfiguration for DNS is enabled (only via registry key)
				var nameServerKey = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{networkInterface.Id}");
				var dnsAutoconfigurationEnabled = nameServerKey?.GetValue("NameServer") != null && string.IsNullOrEmpty(nameServerKey.GetValue("NameServer")?.ToString());

				var listDNSServer = new List<IPAddress>();

				foreach (var dnsServerIPAddress in ipProperties.DnsAddresses)
				{
					listDNSServer.Add(dnsServerIPAddress);
				}

				// Check if IPv4 protocol is available
				var ipv4ProtocolAvailable = true;
				IPv4InterfaceProperties? ipv4Properties = null;

				try
				{
					ipv4Properties = ipProperties.GetIPv4Properties();
				}
				catch (NetworkInformationException)
				{
					ipv4ProtocolAvailable = false;
				}

				listNetworkInterfaceInfo.Add(new NetworkInterfaceInfo
				{
					Id = networkInterface.Id,
					Name = networkInterface.Name,
					Description = networkInterface.Description,
					Type = networkInterface.NetworkInterfaceType.ToString(),
					PhysicalAddress = networkInterface.GetPhysicalAddress(),
					Status = networkInterface.OperationalStatus,
					IsOperational = networkInterface.OperationalStatus == OperationalStatus.Up,
					Speed = networkInterface.Speed,
					IPv4ProtocolAvailable = ipv4ProtocolAvailable,
					IPv4Address = listIPv4Address.ToArray(),
					IPv4Gateway = listIPv4Gateway.ToArray(),
					DhcpEnabled = ipv4Properties != null && ipv4Properties.IsDhcpEnabled,
					DhcpServer = listDhcpServer.ToArray(),
					DhcpLeaseObtained = dhcpLeaseObtained,
					DhcpLeaseExpires = dhcpLeaseExpires,
					IPv6ProtocolAvailable = false,
					IPv6AddressLinkLocal = listIPv6AddressLinkLocal.ToArray(),
					IPv6Address = listIPv6Address.ToArray(),
					IPv6Gateway = listIPv6Gateway.ToArray(),
					DNSAutoconfigurationEnabled = dnsAutoconfigurationEnabled,
					DNSSuffix = ipProperties.DnsSuffix,
					DNSServer = listDNSServer.ToArray()
				});
			}

			return listNetworkInterfaceInfo;
		}

		public static Task<IPAddress?> DetectLocalIPAddressBasedOnRoutingAsync(IPAddress remoteIPAddress)
		{
			return Task.Run(() => DetectLocalIPAddressBasedOnRouting(remoteIPAddress));
		}

		public static IPAddress? DetectLocalIPAddressBasedOnRouting(IPAddress remoteIPAddress)
		{
			using (var socket = new Socket(remoteIPAddress.AddressFamily == AddressFamily.InterNetwork ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp))
			{
				// return null on error...
				try
				{
					socket.Bind(new IPEndPoint(IPAddress.Any, 0));
					socket.Connect(new IPEndPoint(remoteIPAddress, 0));

					if (socket.LocalEndPoint is IPEndPoint ipAddress)
						return ipAddress.Address;
				}
				catch (SocketException) { }
			}

			return null;
		}

		public static Task<IPAddress?> DetectGatewayBasedOnLocalIPAddressAsync(IPAddress localIPAddress)
		{
			return Task.Run(() => DetectGatewayBasedOnLocalIPAddress(localIPAddress));
		}

		public static IPAddress? DetectGatewayBasedOnLocalIPAddress(IPAddress localIPAddress)
		{
			foreach (var networkInterface in GetNetworkInterfaces())
			{
				if (networkInterface == null) continue;
				if (localIPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					if (networkInterface.IPv4Address?.Any(x => x.Item1.Equals(localIPAddress)) ?? false)
					{
						return networkInterface.IPv4Gateway?.FirstOrDefault();
					}
				}
				else if (localIPAddress.AddressFamily == AddressFamily.InterNetworkV6)
				{
					if (networkInterface.IPv6Address?.Contains(localIPAddress) ?? false)
					{
						return networkInterface.IPv6Gateway?.FirstOrDefault();
					}
				}
				else
				{
					throw new Exception("IPv4 or IPv6 address is required to detect the gateway.");
				}
			}

			return null;
		}

		public Task ConfigureNetworkInterfaceAsync(NetworkInterfaceConfig config)
		{
			return Task.Run(() => ConfigureNetworkInterface(config));
		}

		public void ConfigureNetworkInterface(NetworkInterfaceConfig config)
		{
			// IP
			var command = @"netsh interface ipv4 set address name='" + config.Name + @"'";
			command += config.EnableStaticIPAddress ? @" source=static address=" + config.IPAddress + @" mask=" + config.Subnetmask + @" gateway=" + config.Gateway : @" source=dhcp";

			// DNS
			command += @";netsh interface ipv4 set DNSservers name='" + config.Name + @"'";
			command += config.EnableStaticDNS ? @" source=static address=" + config.PrimaryDNSServer + @" register=primary validate=no" : @" source=dhcp";
			command += (config.EnableStaticDNS && !string.IsNullOrEmpty(config.SecondaryDNSServer)) ? @";netsh interface ipv4 add DNSservers name='" + config.Name + @"' address=" + config.SecondaryDNSServer + @" index=2 validate=no" : "";

			try
			{
				PowerShellHelper.ExecuteCommand(command, true);
			}
			catch (Win32Exception win32Ex)
			{
				switch (win32Ex.NativeErrorCode)
				{
					case 1223:
						OnUserHasCanceled();
						break;

					default:
						throw;
				}
			}
		}

		public static Task FlushDnsAsync()
		{
			return Task.Run(() => FlushDns());
		}

		public static void FlushDns()
		{
			const string command = @"ipconfig /flushdns";

			PowerShellHelper.ExecuteCommand(command);
		}

		public static Task ReleaseRenewAsync(IPConfigReleaseRenewMode mode)
		{
			return Task.Run(() => ReleaseRenew(mode));
		}

		public static void ReleaseRenew(IPConfigReleaseRenewMode mode)
		{
			if (mode == IPConfigReleaseRenewMode.ReleaseRenew || mode == IPConfigReleaseRenewMode.Release)
			{
				const string command = @"ipconfig /release";

				PowerShellHelper.ExecuteCommand(command);
			}

			if (mode == IPConfigReleaseRenewMode.ReleaseRenew || mode == IPConfigReleaseRenewMode.Renew)
			{
				const string command = @"ipconfig /renew";

				PowerShellHelper.ExecuteCommand(command);
			}
		}

		public static Task AddIPAddressToNetworkInterfaceAsync(NetworkInterfaceConfig config)
		{
			return Task.Run(() => AddIPAddressToNetworkInterface(config));
		}

		public static void AddIPAddressToNetworkInterface(NetworkInterfaceConfig config)
		{
			var command = @"netsh interface ipv4 add address '" + config.Name + @"' " + config.IPAddress + @" " + config.Subnetmask;

			PowerShellHelper.ExecuteCommand(command, true);
		}

		public static Task RemoveIPAddressFromNetworkInterfaceAsync(NetworkInterfaceConfig config)
		{
			return Task.Run(() => RemoveIPAddressFromNetworkInterface(config));
		}

		public static void RemoveIPAddressFromNetworkInterface(NetworkInterfaceConfig config)
		{
			var command = @"netsh interface ipv4 delete address '" + config.Name + @"' " + config.IPAddress;

			PowerShellHelper.ExecuteCommand(command, true);
		}

		#endregion Methods

		#region Enum

		public enum IPConfigReleaseRenewMode
		{
			ReleaseRenew,
			Release,
			Renew
		}

		#endregion Enum
	}
}