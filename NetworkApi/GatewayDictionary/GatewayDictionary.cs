using IpSwitch.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace NetworkApi.Network.GatewayDictionary
{
	public class GatewayDictionary
	{
		public List<IpToNetwork> HasGatewayIp { get; set; }

		public GatewayDictionary()
		{
			var list = NetworkAdapter.NetWorkList();
			HasGatewayIp = list.Where(i => i.GetIPProperties().GatewayAddresses.Any()).Select(i => new IpToNetwork()
			{
				Network = i,
				NetworkObj = NetworkAdapter.GetNetworkByName(i.Description),
				Ip = i.GetIPProperties().GatewayAddresses.FirstOrDefault()?.Address.ToString(),
				Mac = i.GetPhysicalAddress().ToString(),
				Type = i.NetworkInterfaceType.ToString()
			}).ToList();
		}
	}

	public class IpToNetwork
	{
		public string Ip { get; set; }
		public string Mac { get; set; }
		public string Type { get; set; }
		public NetworkInterface Network { get; set; }
		public ManagementObject NetworkObj { get; set; }
	}
}