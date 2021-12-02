using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkApi.Network.PingDetector
{
	public class IpConfig
	{
		public IpConfig(string ip, bool enable, string port, string? name = null, string? description = null)
		{
			Ip = ip;
			Enable = enable;
			Port = port;
			Name = name;
			Description = description;
		}

		public string Ip { get; set; }
		public bool Enable { get; set; }
		public string Port { get; }
		public string? Name { get; set; }
		public string? Description { get; }
	}
}