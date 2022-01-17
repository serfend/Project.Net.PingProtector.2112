using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApi.NetworkInterfaceManagement
{
	public class NetworkInterfaceConfig
	{
		public string? Name { get; set; }
		public bool EnableStaticIPAddress { get; set; }
		public string? IPAddress { get; set; }
		public string? Subnetmask { get; set; }
		public string? Gateway { get; set; }
		public bool EnableStaticDNS { get; set; }
		public string? PrimaryDNSServer { get; set; }
		public string? SecondaryDNSServer { get; set; }
		public IpVersionConfig IpVersion { get; set; }
	}

	public enum IpVersionConfig
	{
		ipv4 = 0,
		ipv6 = 1
	}
}