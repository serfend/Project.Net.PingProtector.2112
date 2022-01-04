using NetworkApi.NetworkManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRCommunicator.proto
{
	public  class ClientDeviceInfoDTO
	{
		public ClientNetworkInfoDTO? Network { get; set; }
		public ClientComputerInfoDTO? Computer { get; set; }
	}
	public class ClientComputerInfoDTO
	{
		public string? MachineName { get; set; }
		public string? UserName { get; set; }
		public string? OsVersion { get; set; }
		public string? Version { get; set; }
		public long TicketCount { get; set; }
	}
	public class ClientNetworkInfoDTO
	{
		public IEnumerable<NetworkInterfaceDTO?>? Interfaces { get; set; }
	}
}
