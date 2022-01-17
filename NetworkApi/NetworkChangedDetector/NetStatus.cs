using NetworkApi.Models;

namespace NetworkApi.Network.NetworkChangedDetector
{
	public class NetStatus
	{
		/// <summary>
		/// host / gateway / internet
		/// </summary>
		public NetType Type { get; set; }

		public bool IsCheckRunning { get; set; }
		public bool IsCheckComplete { get; set; }
		public string? Status { get; set; }

		/// <summary>
		/// network time log , if -1 then means not reachable
		/// </summary>
		public long Log { get; set; }

		public ConnectionState ConnectionState { get; set; }
		public string? IPAddress { get; set; }
		public string? Hostname { get; set; }
		public const long NOT_Reachable = -1;
	}
}