using NetworkApi.Network.PingDetector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.DAL.Entity.CommonConfig.Server
{
	public class ServerRecordModel : BaseEntity
	{
		public List<IpConfig> Servers { get; set; } = new List<IpConfig>();
		public Dictionary<string, ServerMatchModel> Match { get; set; } = new Dictionary<string, ServerMatchModel>();

		public override object DefaultValue()
		{
			return new ServerRecordModel()
			{
				Match = new Dictionary<string, ServerMatchModel>
				{
					{"bgw",new (){
						Fetcher = new (){ "bgw" },
						Inner = new (){"bgw"},
						Outer = new (){ "gw","jzw" }
					}
					},
					{"csw",new (){
						Fetcher = new (){ "csw" },
						Inner = new (){"csw"},
						Outer = new (){ "bgw","jzw" }
					} },
					{"gw",new (){
						Fetcher = new (){ "gw" },
						Inner = new (){"gw"},
						Outer = new (){ "bgw","jzw" }
					} },
					{"jzw",new (){
						Fetcher = new (){ "jzw" },
						Inner = new (){"jzw"},
						Outer = new (){ "gw","bgw" }
					} },
				},
				Servers = new List<IpConfig>() {
					new  IpConfig(){ Ip = "serfend.top",Port = "443",Enable = true,Name = "gw"},
					new  IpConfig(){ Ip = "192.168.8.196",Port = "2333",Enable = true,Name = "csw"},
					new  IpConfig(){ Ip = "192.168.8.8",Port = "443",Enable = true,Name = "bgw"},
					new  IpConfig(){ Ip = "21.176.51.60",Port = "443",Enable = true,Name = "jzw"},
				}
			};
		}
	}

	public class ServerMatchModel
	{
		public List<string> Fetcher { get; set; } = new List<string>();
		public List<string> Outer { get; set; } = new List<string>();
		public List<string> Inner { get; set; } = new List<string>();
	}

	public static class ServerMatchModelExtensions
	{
		public static List<IpConfig> ToServers(this IEnumerable<string> list, List<IpConfig> servers) => list
				.Distinct()
				.Select(u =>
				{
					var s = servers.FirstOrDefault(i => i.Name == u);
					return s;
				})
				.Where(u => u != null)
				.Select(u => u ?? new IpConfig())
				.ToList();
	}
}